# CONTEXT.md

Domain language and architecture reference for this repo. Written from the Report 1–3 design docs (`docs/*.docx`) before implementation — treat it as the target design, not verified-against-code fact, until the corresponding code lands. Update it as real decisions diverge from the design docs.

## Project

**Restaurant & Table Management System (POS)** — a Windows desktop app (C#, .NET 10, WPF, MVVM) backed by SQL Server via EF Core. Digitizes the restaurant workflow: table floor map → order taking → kitchen display → payment/checkout → inventory deduction → sales reporting. Runs on staff PCs at fixed stations (host stand, cashier counter, kitchen monitor); no mobile/tablet support.

Report 2 §1 specified .NET 8 — see [ADR-0003](docs/adr/0003-dotnet-10.md) for why this repo runs .NET 10 instead.

## Roles

- **Admin** — full access, configuration, reports.
- **Cashier** — tables, orders, payment.
- **Kitchen Staff** — order preparation queue.
- **Manager** — consumes dashboard/report data (indirect; not a login-scoped role in the same sense as the other three).

## Domain glossary

- **RestaurantTable** — a physical table; carries `TableStatus` (Free / Occupied / AwaitingPayment — per Report 1 §3.2 and the SE1919 role doc; no reservation/pre-booking concept exists in the spec). 1—N `Order`.
- **Order** — one dining session at a table. 1—N `OrderItem`, 1—0..1 `Payment`. Carries `OrderStatus`.
- **OrderItem** — a line item within an `Order`. Composition: cannot exist without its parent `Order` — enforced in code by exposing `OrderItems` only through methods on `Order`, no public setter.
- **MenuCategory** — groups `MenuItem`s for UI display (tabs). 1—N `MenuItem`.
- **MenuItem** — a sellable dish/drink with price and availability. 1—N `OrderItem`, N—N `Ingredient` via `MenuItemIngredient`.
- **Ingredient** — a raw stock item tracked by quantity/unit.
- **MenuItemIngredient** — explicit join entity between `MenuItem` and `Ingredient`; carries the recipe's quantity/unit, which is what makes automatic stock deduction possible.
- **Payment** — the checkout record that closes an `Order`. Strict 1-to-0..1 with `Order` — an order is paid at most once.
- **Shift** — tracks a cashier's working session and cash reconciliation. N—1 `User`.
- **User** — login credentials + role for a staff member. 1—N `Order`, `Payment`, `Shift`.

`TableStatus` and `OrderStatus` are modeled as C# enums, not free-text strings — this removes a class of invalid-state bugs at compile time. Don't reintroduce string-typed status fields.

## Architecture (layered)

```
BusinessObjects   → plain entity classes (Order, MenuItem, Ingredient, ...)
DataAccessObjects → DbContext + EF Core config (DAOs)
Repositories      → one repository interface+impl per aggregate, wraps the DAO
Services          → business logic, orchestrates repositories
WpfApp            → Views (XAML) + MVVM/ (RelayCommand, ViewModelBase) + per-screen ViewModels
```

Reference implementation for this exact layering and code style: the local `ProductManagementDemoEF_MVVM` project (`BusinessObjects`/`DataAccessObjects`/`Repositories`/`Services`/`WpfApp`, with `WpfApp/MVVM/{RelayCommand,ViewModelBase}`). Its style: constructors instantiate their own dependency directly (`new CategoryRepository()` — no DI container), CRUD methods return `bool`/`List<T>` directly with no extra wrapping. Match this level of simplicity in this repo unless an ADR says otherwise — don't introduce a DI container, generic repository base class, or result-wrapper type that the reference project doesn't have.

ViewModels expose `ObservableCollection<T>` for bound collections (table grid, cart); UI actions bind to commands (`Send to Kitchen`, `Checkout`), not code-behind event handlers, so business logic stays testable independent of the UI.

**Resolved naming decision**: Report 1's proposal-stage solution layout named four projects `RestaurantPOS.Views` / `RestaurantPOS.Controllers` / `RestaurantPOS.BLL` / `RestaurantPOS.DAL`, calling the architecture "MVC". Report 3 (what was actually built later) describes MVVM instead — ViewModels, `ObservableCollection<T>`, command bindings, no code-behind business logic. Decision: use real MVVM (ViewModels, not Controllers) — Report 3 is the as-built record and this is the correct WPF idiom. Solution project names:

```
RestaurantPOS.BusinessObjects
RestaurantPOS.DataAccessObjects
RestaurantPOS.Repositories
RestaurantPOS.Services          (= "BLL" in the docs)
RestaurantPOS.WpfApp             (Views + MVVM/{RelayCommand,ViewModelBase} + ViewModels)
```

## Tech stack (decided, from Report 1 §6.2)

- **MaterialDesignThemes** (MaterialDesignInXAML Toolkit) — WPF UI shell, components, theming.
- **LiveCharts2.WPF** — dashboard revenue charts.
- **Microsoft.EntityFrameworkCore.SqlServer** — ORM, Code-First.
- **FastReport.OpenSource** — invoice/report generation and printing.
- IDE: Visual Studio 2022. DB: SQL Server 2022 Developer + SSMS. Target OS: Windows 10/11 64-bit.

## Explicitly out of scope (from Report 1 §4.2)

Online ordering / mobile app, multi-branch management, cross-device real-time networking beyond a single machine or LAN (no push protocol — this is *why* the kitchen display polls instead of using SignalR/WebSocket).

## Git / CI conventions (from Report 2 §5.1)

- Branch naming: `feature/<module-name>`. PR review required from the Team Lead before merge to `main` — no direct pushes.
- GitHub Actions (`ci.yml`) on every PR to `main`: `dotnet build` (windows-latest), `dotnet test` (xUnit project `RestaurantPOS.Tests`), `dotnet format --verify-no-changes`.
- On merge to `main`: release workflow runs `dotnet publish`, produces a `.zip` artifact. The upload-to-GitHub-Releases step was removed (see `release.yml`, commit `ac350f3`) — no release is published automatically anymore.
- Risk-driven rule (Report 1 §7): no business logic in View code-behind — this is the concrete rule that keeps the MVVM boundary real, not just aspirational.

## Key design decisions (from Report 2)

- EF Core Code-First with Fluent API for relationships plain conventions can't express:
  - `Order`–`Payment`: required 1-to-0..1 via a unique FK on `Payments.OrderId`.
  - `MenuItemIngredient`: modeled as an explicit join entity (not an implicit many-to-many), so recipe quantity/unit can live on the relationship itself.
  - Cascade delete restricted on `RestaurantTable`→`Order` and `Order`→`OrderItem`, to avoid accidental bulk deletion.
- Kitchen display polls pending `OrderItem`s every 10 seconds via a background `async Task` + `Task.Delay` (not `DispatcherTimer`), so querying never blocks the UI thread; results are marshalled back via `Dispatcher.Invoke`/the async context before updating bound collections.

## Known trade-offs / open risk areas (from Report 3)

- **Polling, not push** (no SignalR/WebSocket) → kitchen screen can lag up to 10s behind a new order. Accepted deliberately for LAN-only, single-restaurant scale — don't "fix" this by adding a push mechanism without discussing it first.
- **Concurrent cashier edits** on the same order: resolved. `Orders.RowVersion` is a concurrency token (`AppDbContext.cs`); `PaymentDAO.CheckoutOrder` catches `DbUpdateConcurrencyException` and rejects the stale update rather than overwriting it.
- **Payment + inventory deduction is one DB transaction**: resolved. `PaymentDAO.CheckoutOrder` adds the `Payment`, decrements every `Ingredient.QuantityInStock`, and closes the `Order`/frees the `Table` in a single `SaveChanges()` call — all-or-nothing, no double-deduct window.
- **Double-booking a table**: resolved. `RestaurantTable` has no concurrency token, so instead `OrderDAO.CreateOrder` does a conditional `ExecuteUpdate` (`WHERE TableId=@id AND Status=Free`) inside a transaction — a losing concurrent request gets 0 rows affected instead of creating a second `Order` on the same table.
- Open gaps as of Report 3, still unresolved: receipt printing (FastReport.OpenSource is referenced in `RestaurantPOS.WpfApp.csproj` but not called from any code yet) and centralized error logging (DAOs catch and swallow exceptions with a `// Log the exception (ex) as needed` comment — no `ILogger`/logging framework wired up).
- Low-stock threshold: resolved. `Ingredient.LowStockThreshold` + `IsLowStock` are used by `DashboardService.GetLowStockIngredients()`, surfaced in `DashboardViewModel.LowStockIngredients` and shown on `DashboardWindow` — the dashboard's `«include»` low-stock check is wired end to end.
- Password hashing resolved: PBKDF2 via `Rfc2898DeriveBytes`, no new dependency — see [ADR-0002](docs/adr/0002-password-hashing-pbkdf2.md).

## Status

Fully implemented: `RestaurantPOS.BusinessObjects`, `RestaurantPOS.DataAccessObjects`, `RestaurantPOS.Repositories`, `RestaurantPOS.Services`, `RestaurantPOS.WpfApp`, `RestaurantPOS.Tests`. This file describes the target design from Report 1–3; it has been checked against the actual code and updated where they diverged (see the resolved trade-offs above). Remaining known gaps: receipt printing, centralized error logging.
