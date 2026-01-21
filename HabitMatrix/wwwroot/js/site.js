// =============================
// HabitMatrix - site.js
// =============================
// Purpose: Handles all client-side interactions for Habit Board, Add Habit modal, Suggestions, Theme toggle, and Dashboard charts.
// =============================
console.log("site.js loaded");

/* --------------------------------------------------------------------------
 * Utility Functions
 * -------------------------------------------------------------------------- */

/**
 * POST form helper
 * Centralizes fetch + antiforgery token handling
 * Returns JSON or empty object if parsing fails
 */
async function postForm(url, data) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    const res = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token
        },
        body: new URLSearchParams(data)
    });
    if (!res.ok) throw new Error(`Request failed: ${res.status}`);
    return res.json().catch(() => ({}));
}

/**
 * GET JSON helper
 * Simplifies repeated fetch + json parsing
 */
async function getJSON(url) {
    const res = await fetch(url);
    if (!res.ok) throw new Error(`GET failed: ${res.status}`);
    return res.json();
}

/* Toggle habit log */
async function toggleHabit(btn) {
    try {
        const { habit, date, status } = btn.dataset;
        const data = await postForm('/Habit/ToggleLog', { habitId: habit, logDate: date, status });
        const completed = data.status ?? (status === "true");

        // Update UI state
        btn.textContent = completed ? "✔" : "✘";
        btn.className = `px-2 py-1 rounded ${completed ? "bg-green-500 text-white" : "bg-red-500 text-white"}`;
        btn.dataset.status = completed ? "false" : "true";
    } catch (err) {
        console.error("Toggle error:", err);
        alert(err.message);
    }
}

/* Add habit */
async function addHabit(e) {
    e.preventDefault();
    try {
        const category = document.getElementById('category')?.value;
        const habitName = document.getElementById('habitName')?.value;
        const habitColor = document.getElementById('habitColor')?.value;
        const newHabit = await postForm('/Habit/AddHabit', { category, habitName, habitColor });

        const table = document.querySelector('#habitTable tbody');
        if (!table) return;

        const daysInMonth = new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate();
        const row = document.createElement('tr');

        // Render new habit row with buttons for each day
        row.innerHTML = `
            <td class="border px-4 py-2 font-semibold" style="color:${newHabit.color}">${newHabit.name}</td>
            ${Array.from({ length: daysInMonth }, (_, i) => `
                <td class="border px-2 py-1 text-center">
                    <button type="button"
                            class="px-2 py-1 rounded bg-red-500 text-white"
                            data-habit="${newHabit.id}"
                            data-date="${new Date().getFullYear()}-${String(new Date().getMonth() + 1).padStart(2, '0')}-${String(i + 1).padStart(2, '0')}"
                            data-status="true">✘</button>
                </td>`).join('')}
        `;
        table.appendChild(row);

        // Close modal after adding
        document.getElementById('addHabitModal')?.classList.add('hidden');
    } catch (err) {
        console.error("Add habit error:", err);
        alert(err.message);
    }
}

/* Load categories */
async function loadCategories() {
    try {
        const categories = await getJSON('/Habit/Categories');
        const categorySelect = document.getElementById('category');
        if (categorySelect) {
            categorySelect.innerHTML = categories.map(c => `<option value="${c.id}">${c.name}</option>`).join('');
        }
    } catch (err) {
        console.error("Category load error:", err);
    }
}

/* --------------------------------------------------------------------------
 * Event Bindings (DOMContentLoaded)
 * -------------------------------------------------------------------------- */
// NOTE: Previously you had two separate DOMContentLoaded blocks.
//       Now merged into one for clarity and efficiency.
document.addEventListener("DOMContentLoaded", () => {
    // -------------------------------
    // Habit Suggestions
    // -------------------------------
    const categoryEl = document.getElementById('category');
    const suggestionBox = document.getElementById('habitSuggestions');
    const habitNameEl = document.getElementById('habitName');

    if (categoryEl && suggestionBox) {
        categoryEl.addEventListener('change', async () => {
            try {
                const suggestions = await getJSON(`/Habit/Suggestions?category=${encodeURIComponent(categoryEl.value)}`);
                suggestionBox.innerHTML = suggestions.map(s => `<li class="cursor-pointer hover:bg-gray-100 p-1">${s}</li>`).join('');
                suggestionBox.querySelectorAll('li').forEach(li => {
                    li.addEventListener('click', () => habitNameEl && (habitNameEl.value = li.textContent));
                });
            } catch (err) { console.error("Suggestion error:", err); }
        });
    }

    if (habitNameEl && categoryEl && suggestionBox) {
        habitNameEl.addEventListener('input', async () => {
            try {
                const suggestions = await getJSON(`/Habit/Suggestions?categoryId=${encodeURIComponent(categoryEl.value)}&query=${encodeURIComponent(habitNameEl.value)}`);
                suggestionBox.innerHTML = suggestions.map(s => `<option value="${s}">`).join('');
            } catch (err) { console.error("Suggestion error:", err); }
        });
    }

    // -------------------------------
    // Add Habit Modal
    // -------------------------------
    const addHabitBtn = document.getElementById('addHabitBtn');
    const closeModalBtn = document.getElementById('closeModal');
    const addHabitForm = document.getElementById('addHabitForm');

    addHabitBtn?.addEventListener('click', () => {
        loadCategories();
        document.getElementById('addHabitModal')?.classList.remove('hidden');
    });

    closeModalBtn?.addEventListener('click', () => {
        document.getElementById('addHabitModal')?.classList.add('hidden');
    });

    addHabitForm?.addEventListener('submit', addHabit);

    // -------------------------------
    // Habit button clicks (delegated)
    // -------------------------------
    document.addEventListener('click', e => {
        const btn = e.target.closest('button[data-habit]');
        if (btn) toggleHabit(btn);
    });

    // -------------------------------
    // Theme Toggle (using hidden field)
    // -------------------------------
    console.log("site.js loaded");

    document.addEventListener("DOMContentLoaded", function () {
        console.log("DOM ready");

        const toggleBtn = document.getElementById("themeToggle");
        const themeLink = document.querySelector("link[href*='site']");
        const themeHidden = document.getElementById("themePrefHidden"); // hidden field in _Layout.cshtml
        const themeIcon = document.getElementById("themeIcon"); // sun/moon icon span

        if (!toggleBtn) {
            console.error("themeToggle button not found in DOM");
            return;
        }

        toggleBtn.addEventListener("click", function () {
            console.log("Theme toggle clicked");

            // ✅ Read current theme from hidden field
            const current = themeHidden?.value || "light";
            const newTheme = current === "dark" ? "light" : "dark";
            console.log("New theme:", newTheme);

            const newHref = `/css/${newTheme === "dark" ? "site-dark.css" : "site.css"}`;
            console.log("Swapping to:", newHref);

            if (themeLink) {
                themeLink.href = newHref;
                console.log("Link updated:", themeLink.href);
            } else {
                console.error("Theme link not found");
            }

            // ✅ Persist to backend (DB/claims)
            fetch("/Account/SaveTheme", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ theme: newTheme })
            })
                .then(r => console.log("SaveTheme status:", r.status))
                .catch(e => console.error("SaveTheme error:", e));

            // ✅ Update hidden field so frontend stays in sync
            if (themeHidden) themeHidden.value = newTheme;

            // ✅ Update icon instantly
            if (themeIcon) themeIcon.textContent = newTheme === "dark" ? "☀️" : "🌙";
        });
    });
//    const toggleBtn = document.getElementById("themeToggle");
//    const themeLink = document.querySelector("link[href*='site']");
//    const themeHidden = document.getElementById("themePrefHidden"); // hidden field in _Layout.cshtml
//    const themeIcon = document.getElementById("themeIcon"); // sun/moon icon span

//    toggleBtn?.addEventListener("click", () => {
//        const current = themeHidden?.value || "light";
//        const newTheme = current === "dark" ? "light" : "dark";
//        const newHref = `/css/${newTheme === "dark" ? "site-dark.css" : "site.css"}`;

//        if (themeLink) themeLink.href = newHref;

//        // Persist to backend
//        fetch("/Account/SaveTheme", {
//            method: "POST",
//            headers: { "Content-Type": "application/json" },
//            body: JSON.stringify({ theme: newTheme })
//        }).catch(e => console.error("SaveTheme error:", e));

//        // Update hidden field so frontend stays in sync
//        if (themeHidden) themeHidden.value = newTheme;

//        // Update icon instantly
//        if (themeIcon) themeIcon.textContent = newTheme === "dark" ? "☀️" : "🌙";
//    });
//});
/* --------------------------------------------------------------------------
 * Dashboard Visualizations (Chart.js)
 * -------------------------------------------------------------------------- */
// NOTE: Added a helper `renderChart` to avoid repeating element checks.
//       Each chart initializer now calls this helper, keeping code DRY and safe.

function renderChart(id, config) {
    const el = document.getElementById(id);
    if (!el) return; // Guard: prevents errors if element not present
    new Chart(el, config);
}

/* Weekly Completion Rate Chart */
function initWeeklyRateChart(rate = 0) {
    renderChart('weeklyRateChart', {
        type: 'doughnut',
        data: {
            labels: ['Completed', 'Missed'],
            datasets: [{
                data: [rate, 100 - rate],
                backgroundColor: ['#10b981', '#ef4444']
            }]
        },
        options: {
            responsive: true,
            plugins: { legend: { display: false } }
        }
    });
}

/* Habit Completion Trend Chart */
function initTrendChart(labels = [], completed = [], missed = []) {
    renderChart('habitTrendChart', {
        type: 'line',
        data: {
            labels,
            datasets: [
                {
                    label: 'Completed',
                    data: completed,
                    borderColor: '#10b981',
                    backgroundColor: 'rgba(16,185,129,0.2)',
                    fill: true
                },
                {
                    label: 'Missed',
                    data: missed,
                    borderColor: '#ef4444',
                    backgroundColor: 'rgba(239,68,68,0.2)',
                    fill: true
                }
            ]
        },
        options: {
            responsive: true,
            scales: { y: { beginAtZero: true } }
        }
    });
}

/* Habit Category Distribution Chart */
function initCategoryChart(labels = [], values = []) {
    const colors = ["#3b82f6", "#10b981", "#f59e0b", "#ef4444", "#6366f1"];
    renderChart('habitCategoryChart', {
        type: 'pie',
        data: {
            labels,
            datasets: [{
                data: values,
                backgroundColor: colors.slice(0, labels.length)
            }]
        },
        options: { responsive: true }
    });
}

/* Habit Streaks Chart */
function initStreakChart(labels = [], values = []) {
    renderChart('habitStreakChart', {
        type: 'bar',
        data: {
            labels,
            datasets: [{
                label: 'Streak Length',
                data: values,
                backgroundColor: '#6366f1'
            }]
        },
        options: {
            responsive: true,
            scales: { y: { beginAtZero: true } }
        }
    });
}

/* Top Habits Chart */
function initTopHabitsChart(labels = [], values = []) {
    renderChart('topHabitsChart', {
        type: 'bar',
        data: {
            labels,
            datasets: [{
                label: 'Completion %',
                data: values,
                backgroundColor: '#10b981'
            }]
        },
        options: {
            indexAxis: 'y',
            responsive: true,
            scales: { x: { beginAtZero: true, max: 100 } }
        }
    });
}