# User-tester report — RestaurantPOS.WpfApp — 2026-07-30

Full-app QA pass, mystery-shopper style: built and ran the real `.exe`, drove it through UI Automation (PowerShell + `System.Windows.Automation`, no Playwright — this is a WPF desktop app, not a web app), verified against the real `(localdb)\mssqllocaldb` / `RestaurantPOSDb` database, and read the source for every root cause rather than trusting on-screen messages. 76 test-matrix rows run across Admin, Cashier, and KitchenStaff roles plus 5 cross-cutting rows. Every `FAIL` was re-confirmed — either by a second fresh live click-through, or (when a second click-through would add no value) by an independent channel: `error.log`, direct SQL, or a second source-code read. All 9 findings then went through an adversarial fresh-eyes pass (a `code-reviewer` agent tasked to refute each one against the actual source) before ranking.

**Working notes (full 76-row matrix with evidence):** `working-notes.md`, same folder.

## Verdict: **Ship** (for a school/graded project), with a short punch-list

No blocker. No major survived adversarial review — the two findings that looked "major" on first pass (duplicate login windows, and a theoretical DB-timing race around the shift gate) were both downgraded after a second reviewer read the actual enforcement code and found the real-world impact was less than the original write-up claimed. What's left is 9 real, reproducible-or-well-evidenced **minor** bugs — none of them lose data, corrupt money, or block a flow outright. Worth fixing before a real deployment; not worth blocking a school submission over.

## Bugs (all minor after adversarial review — ordered roughly by real-world reachability/impact)

Each entry: original finding → adversarial-review verdict → why the final severity is what it is.

### 1. Ingredient update silently misreports concurrency conflicts, and the failure is invisible to logs
**File:** `RestaurantPOS.DataAccessObjects/IngredientDAO.cs:30-48`, `RestaurantPOS.WpfApp/ViewModels/InventoryViewModel.cs`.
Repro: two staff editing the same ingredient near-simultaneously (simulated via raw SQL bumping `RowVersion` between the Inventory window's `Load()` and a UI-driven update) → the update is correctly rejected (no data corruption), but the UI shows **"Không thể cập nhật nguyên liệu — tên có thể đã tồn tại."** (implies a duplicate name — nonsensical here, the name wasn't touched), and **zero entry is written to `error.log`** for this failure mode: `IngredientDAO.UpdateIngredient` has a dedicated `catch (DbUpdateConcurrencyException) { return false; }` with no `AppLogger` call, distinct from the generic `catch (Exception ex)` right below it that *does* log.
**Adversarial verdict: CONFIRMED minor.** The reject itself is correct (no corruption), so this is an audit-trail/UX gap, not a data-integrity bug — but it's the one finding where a real concurrent-edit scenario in a live restaurant is plausible, and right now it leaves zero trace.
**Fix:** log this catch block too; give it its own message instead of reusing the duplicate-name string.

### 2. Checkout succeeds with 0 order items (₫0 payment) — reachable via the normal flow, not just an edge case
**Files:** `RestaurantPOS.Services/PaymentService.cs`, `OrderWindow.xaml` (`Checkout_Click` has no `CanExecute`/disabled-state guard, unlike "Gửi bếp" which is properly gated on `DraftLines.Count > 0`).
Repro: open a table, click straight through to "Thanh toán" with 0 items — Payment window shows "Tổng cộng: 0 đ", confirms, and a real `Payment` row (`AmountPaid=0`) is created, `Order.Status` flips to Paid, table frees.
**Adversarial verdict: CONFIRMED minor, broader reachability than first thought** — the fresh-eyes pass found this is reachable through the plain happy-path (no items added at all), not only via the recovered-phantom-order path row 6 below describes. Still no financial loss (it's ₫0), but it lets a table close out with a legitimate-looking but empty Payment record.
**Fix:** disable checkout when `OrderItems.Count == 0`, same pattern already used for "Gửi bếp".

### 3. Trailing-space item name defeats duplicate-name validation
**File:** `RestaurantPOS.Services/MenuItemService.cs:39-41` (`IsDuplicateName`) — no `.Trim()` anywhere in the Service or ViewModel.
Repro (reproduced live twice, on fresh sessions both times): adding "Spring Rolls " (trailing space) succeeds even though "Spring Rolls" already exists — visually indistinguishable rows in the DataGrid.
**Adversarial verdict: DOWNGRADED major → minor.** Real and reproducible, but cosmetic: no crash, no functional duplicate (different `MenuItemId`), and it still blocks the far more common exact-retype case the recent commit (`1033c16`) targeted.
**Fix:** `.Trim()` before the comparison and before save.

### 4. Closing the Order window's X button with unsent draft items silently discards them
**File:** `RestaurantPOS.WpfApp/OrderWindow.xaml.cs` — confirmed to have **no** `Closing` event handler at all (contrast with `ShiftWindow.xaml.cs:16`, which does have one).
Repro: add items to a draft, close via the title-bar X with zero prompt — the underlying `Order` row survives with 0 `OrderItems`, table stays Occupied.
**Adversarial verdict: CONFIRMED minor, and genuinely recoverable** — re-opening that table reloads the same (now-empty) order via `TableMapViewModel.OpenOrderForTable` → `GetOpenOrderByTable`, and the existing "Hủy đơn" (cancel order) command frees it properly. Not a dead-end, just easy to miss.
**Fix:** add a `Closing` guard mirroring `ShiftWindow`'s pattern when there are unsent draft lines.

### 5. Negative shift opening cash accepted with no validation
**Files:** `RestaurantPOS.WpfApp/ViewModels/ShiftViewModel.cs` (`OpenNewShift`), `RestaurantPOS.Services/ShiftService.cs` (`OpenShift`) — neither validates the value, only "no existing open shift."
Repro: opened a shift with `OpeningCash = -100` — reconciliation view then shows "-100 đ" as the whole shift's cash baseline. Confirmed the `TextBox` binding has no input mask/converter, so a real keyboard user typing `-100` reaches this directly (not a UI-Automation-only artifact).
**Adversarial verdict: CONFIRMED minor** — cosmetic reconciliation-math glitch, no security/financial-loss angle, recoverable at shift close.
**Fix:** reject `OpeningCash <= 0` the same way `MenuItemViewModel` already rejects `Price <= 0`.

### 6. 10,000-character item name shows a misleading "duplicate name" error
**Files:** `MenuItemDAO.cs:27-41` (catches generic `Exception`, logs it, returns `false`), `MenuManagementViewModel.cs` (maps every `false` to the hardcoded duplicate-name message).
Real cause (confirmed independently via `error.log`, not just the on-screen text): `ItemName` is `nvarchar(150)`; the oversized insert throws a truncation `SqlException`, mislabeled on screen as a duplicate-name rejection.
**Adversarial verdict: CONFIRMED minor**, likelihood judged low (no `MaxLength` on the TextBox, so only a large clipboard paste would trigger it in practice) — but the same generic-catch-then-hardcoded-message template repeats across at least two DAOs (this one and Ingredient's, finding #1), so it's a systemic pattern worth a dev-side note even though only two instances were chased down given time budget.
**Fix:** stop collapsing every DAO failure into one hardcoded UI string; at minimum distinguish "too long" from "duplicate."

### 7. Double-click on Login can open two MainWindow/ShiftWindow sessions
**File:** `RestaurantPOS.WpfApp/LoginWindow.xaml.cs` — `Login_Click` disables the button, but a `finally` re-enables it even on the success path, right as `Close()` tears the window down.
**Adversarial verdict: DOWNGRADED major → minor/polish.** Reproduced live twice via `InvokePattern.Invoke()` called twice back-to-back — but the reviewer's re-read of the code makes a strong case this is a **test-tooling artifact, not a reachable real-user bug**: `Login_Click` is fully synchronous with no yield point, so a genuine mouse double-click's second event is only dispatched by WPF after the handler returns — by which point `Close()` has already run and the window is gone; a real second click can't land on a button that no longer exists the way a cached `InvokePattern` reference can. `SessionContext.CurrentUser` is also just single-user shared state, not a cross-session integrity risk. Still worth a cleanup (the `finally` re-enable is dead weight once `Close()` is about to fire on the success path) but not a real user-facing risk.

### 8. Keyboard-only navigation can't reach any table tile in TableMapWindow
**File:** `RestaurantPOS.WpfApp/Views/TableMapWindow.xaml` — table tiles are `<Border MouseLeftButtonDown="...">`, not `Button`, with no `Focusable`, no `KeyDown`, no tab stop.
Confirmed live by literally Tab-stepping through the window: focus only ever cycles between the "Làm mới" button and one unnamed `List` container, never an individual tile — a keyboard-only user cannot select a table at all, blocking Order/Payment entirely without a mouse.
**Adversarial verdict: DOWNGRADED major → minor/polish.** The technical claim is correct and live-confirmed, but the reviewer checked `CONTEXT.md` and every ADR in the repo and found no accessibility requirement stated anywhere — this project is explicitly scoped as a fixed-station staff desktop app. Also worth knowing if this is ever picked up: even a focused tile wouldn't help today, since the click handler requires `ClickCount == 2` with no keyboard-equivalent — this is a from-scratch feature gap, not a broken existing one.

### 9. DB blip at exactly the post-login shift-gate moment could skip showing the gate — not live-reproduced, and its claimed impact doesn't hold up
**File:** `RestaurantPOS.WpfApp/MainWindow.xaml.cs:22-36` (`MainWindow_ContentRendered`).
Original claim: `_shiftGateShown = true` is latched before the (try/catch-free) DB call; if that call throws, the app-global handler catches it and survives, but the gate dialog never appears again that session — I read this as potentially letting a Cashier process cash payments without ever opening a shift. **Never reproduced live** — the race window is sub-100ms, out of reach for tool calls with process/network round-trip overhead; flagged from the start as code-derived only, corroborated by the developer's own comment acknowledging the risk.
**Adversarial verdict: DOWNGRADED, and the business-impact claim is specifically refuted.** The code-level trace holds (confirmed the whole chain down to `ShiftDAO.GetOpenShift` has no try/catch, and `App.xaml.cs`'s global handler does swallow it gracefully) — but `ADR-0004` and `PaymentService.Checkout` (line 27-38) show cash-payment-requires-open-shift is enforced **independently in the Service layer**, regardless of whether the gate dialog ever showed. So the worst case is a skipped reminder popup, not a bypassed business rule. Downgraded to minor; still worth a try/catch around `RefreshShift()` in `ContentRendered` as cheap insurance, but not urgent.

## UX complaints not captured in the matrix

- **Username lookup is case-insensitive** (DB collation `SQL_Latin1_General_CP1_CI_AS`) — `ADMIN`/`Admin@123` logs in fine. Not a bug, but confirm this is the intended design, not an oversight.
- **Cross-category menu-item name uniqueness is global, not per-category** — two different "Trà đá" in different sections can't both exist. Might be intentional; flagging as a product decision for Boss to confirm either way.
- **No login rate-limiting or lockout** — 5 rapid wrong-password attempts produce the same generic error every time, no delay/lockout. Known gap, not chasing it as a bug given this is a closed staff-only desktop app with no internet exposure by design.
- Menu Management's `AddMenuItemCommand`/`UpdateMenuItemCommand` both reset all input fields (including the category selection) after every successful save — mildly annoying for adding several items in the same category back-to-back, but not incorrect.

## Blocked / not tested

- **Checkout-time stock race between two concurrent orders for the last unit of an ingredient** — genuinely needs two independent, truly concurrent UI sessions; out of reach for this run's single-threaded UI-Automation driver in the time available. Recommend a dedicated concurrency-focused pass if this matters for grading.
- **KitchenStaff role's live mark-in-progress/mark-done interaction** — the kitchen queue was empty by the time `kitchen1` was tested (everything already Done/Paid from prior Admin-session testing). Confirmed the role reaches the Kitchen screen correctly with the right menu visibility, but didn't get a fresh click-through of the mark-done action *as that role specifically* — the same code path was already exercised and confirmed working under the Admin session.

Everything else in the 76-row matrix (`working-notes.md`) reached a `PASS`, `FAIL` (all 9 listed above), or the two `BLOCKED` rows above — nothing silently skipped.

## Test data created (safe to clean up or keep — dev/local DB)

- **Accounts:** `cashier1` / `Cashier@123` (UserId 2, Cashier), `kitchen1` / `Kitchen@123` (UserId 3, KitchenStaff) — created with Boss's permission via the app's own `PasswordHasher` algorithm.
- **Shifts:** 5 rows total. Shift #1 (admin, `OpeningCash=-100`, closed — the negative-cash bug repro, finding #5). Shift #3 (admin, synthetic midnight-spanning row inserted directly via SQL for the date-bucketing test, closed). Shift #2 (admin) and Shift #5 (cashier1) are **currently still open** (`ClosedAt IS NULL`) — left open at the end of this run; close them manually before further use if that matters for grading, or leave them as-is since this is dev data.
- **Orders/Payments:** 6 Orders, 4 Payments, spanning normal checkouts, a bank-transfer payment, and the ₫0 checkout from finding #2.
- **Menu:** 3 categories (Appetizers, Main Course, Beverages), 4 items — back to the original count; all test-only rows (a duplicate "Spring Rolls ", a 10,000-char name) were created and then deleted during testing, confirmed back to baseline.

Share note: this file and `working-notes.md` are the two files this run produced, both under `docs/user-tester/2026-07-30-full-app/`.
