# Frontend Styling & Razor View Rules (frontend-rules.md)

This document establishes development guidelines for creating and modifying ASP.NET Core MVC views, layout configurations, styling sheets, design tokens, and frontend scripts in the Gym Management System. Adhering to these standards ensures visual coherence and a modern UX.

---

## 🎨 Technology Stack & Asset Libraries

-   **HTML Framework:** ASP.NET Core Razor Views (`.cshtml`)
-   **CSS Framework:** Bootstrap 5.3.x
-   **Icon Library:** Bootstrap Icons 1.11.0 (imported via CDN in layouts)
-   **Script Suite:** jQuery, jQuery Validation, and jQuery Unobtrusive Validation
-   **Typography (Google Fonts):**
    -   `Baloo Thambi 2`: Applied globally to all headers (`h1`-`h6`) and the navigation bar.
    -   `Rubik`: Applied to paragraphs, standard text blocks, and pre-formatted text.

---

## 💎 Design System Tokens (CSS Variables)

We define global design values in `wwwroot/css/style.css` inside the `:root` pseudo-class. **Never hardcode hex values** for primary accents or layout timings in new styling classes.

### Custom CSS Variables:
```css
:root {
    --primary-color: #070093;      /* Deep Royal Blue Theme */
    --main-duration: 300ms;       /* Standard transition duration */
    --black-alternative: #242424; /* Dark charcoal text */
}
```

### Utility Class Mapping:
To keep HTML layout clean and minimize custom styles, utilize the pre-built CSS classes mapping to variables:
-   **Primary Text:** `.text-primary-color` (matches `--primary-color`)
-   **Primary Background:** `.bg-primary-color` (matches `--primary-color`)
-   **Primary Border:** `.border-primary-color` (matches `--primary-color`)
-   **Alternative Charcoal Text:** `.text-black-alternative` (matches `--black-alternative`)
-   **Custom Margins:** `.mt-6` / `.mb-6` (maps to `4rem` for larger spacing blocks)
-   **Custom Font Size:** `.fs-18` (standardized `18px` font sizes)

---

## 📦 Razor Layout Structure (`_Layout.cshtml`)

All application pages share a main layout which establishes the skeleton structure. When modifying or creating layouts, preserve this sequential outline:

```
┌────────────────────────────────────────────────────────┐
│  Head: CDN Preconnects, Google Fonts, Bootstrap, CSS  │
├────────────────────────────────────────────────────────┤
│  Body:                                                 │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Header: Responsive Navigation Bar (.navbar)     │  │
│  ├──────────────────────────────────────────────────┤  │
│  │  Main: Container Wrapper & Content Rendering     │  │
│  │        @RenderBody()                             │  │
│  ├──────────────────────────────────────────────────┤  │
│  │  Footer: Copyright & Privacy Links               │  │
│  └──────────────────────────────────────────────────┘  │
├────────────────────────────────────────────────────────┤
│  Scripts: jQuery, Bootstrap Bundle, site.js, Section   │
└────────────────────────────────────────────────────────┘
```

### Dynamic Script Injection:
Never inject scripts in the middle of views. Always push page-specific interactive scripts (e.g., custom chart loaders, validation setups) to the bottom of the body using the Razor scripts section:
```html
@section Scripts {
    <script src="~/js/member-page.js"></script>
    <partial name="_ValidationScriptsPartial" />
}
```
This maps directly to `@await RenderSectionAsync("Scripts", required: false)` in the base layout.

---

## 📏 View Markup Conventions & Best Practices

When coding views, strictly execute the following patterns to keep files clean, maintainable, and highly responsive:

### 1. Strongly Typed Views
Every view must declare its expectation of data at the absolute top of the file:
```html
@model IEnumerable<codewithmena.GymManagementSystem.DAL.Entities.Plan>
```

### 2. ASP.NET Tag Helpers over Hardcoded Links
Avoid raw relative paths like `<a href="/Plans/Details/5">`. Always use framework-native Tag Helpers to ensure routes map correctly even if controllers are renamed:
```html
<a asp-controller="Plans" asp-action="Details" asp-route-id="@plan.Id" class="btn btn-outline btn-sm">
    <i class="bi bi-eye me-1"></i>View Details
</a>
```

### 3. Graceful Empty States
When displaying list collections, always verify that the dataset contains items before looping to build HTML. If no elements are present, render a friendly full-width descriptive card rather than showing an empty blank page:
```html
@if (Model?.Any() == true)
{
    <div class="row g-4">
        @foreach (var item in Model) { ... }
    </div>
}
else
{
    <div class="text-center py-5">
        <i class="bi bi-inbox display-1 text-muted"></i>
        <h4 class="mt-3 text-muted">No Items Available</h4>
    </div>
}
```

### 4. Semantic Markup and Shadow Cards
For modular lists (like Gym Packages, Membership cards, Session schedules), design utilizing borderless, shadowed cards with matching responsive columns:
```html
<div class="col-12 col-md-6 col-lg-4">
    <div class="card h-100 shadow-sm border-0">
        ...
    </div>
</div>
```

### 5. Conditional Class Styling
Leverage inline ternary logic in Razor templates to render status-specific backgrounds or badges instead of creating duplicate view copies:
```html
<!-- Background matches Active/Inactive status dynamically -->
<div class="card-header text-white text-center py-4 @(plan.IsActive ? "bg-primary-color" : "bg-secondary")">
    <h4 class="mb-0">@plan.Name</h4>
</div>
```
```html
<!-- Deactivate / Activate styling shifts -->
<button type="submit" class="btn btn-outline btn-sm w-100 @(plan.IsActive ? "text-danger" : "text-success")">
    <i class="bi @(plan.IsActive ? "bi-x-circle" : "bi-check-circle") me-1"></i>
    @(plan.IsActive ? "Deactivate" : "Activate")
</button>
```
Preserve this clean styling paradigm in all future admin and member dash views.
