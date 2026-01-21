// =============================
// Habit Board Interactions
// =============================

/**
 * Toggle a habit log between ✔ (completed) and ✘ (missed).
 * This sends an AJAX POST to /Habit/ToggleLog with habitId, date, and status.
 * The server responds with JSON {status:true/false}, which we use to update the UI.
 */
console.log("site.js loaded");
async function toggleHabit(btn) {
    const habitId = btn.dataset.habit;   // habit GUID
    const logDate = btn.dataset.date;    // yyyy-MM-dd string
    const status = btn.dataset.status;   // "true" or "false"
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value; // anti-forgery token

    try {
        const res = await fetch('/Habit/ToggleLog', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token
            },
            body: new URLSearchParams({ habitId, logDate, status })
        });

        if (!res.ok) throw new Error("Failed to update habit log");

        let data = {};
        try {
            data = await res.json(); // expect JSON {status:true/false}
        } catch {
            // fallback if server didn’t return JSON
            data.status = (status === "true");
        }

        // Update UI based on returned status
        if (data.status === true) {
            btn.textContent = "✔";
            btn.classList.remove("bg-red-500");
            btn.classList.add("bg-green-500", "text-white");
            btn.dataset.status = "false"; // next toggle will flip
        } else {
            btn.textContent = "✘";
            btn.classList.remove("bg-green-500");
            btn.classList.add("bg-red-500", "text-white");
            btn.dataset.status = "true";
        }
    } catch (err) {
        console.error("Toggle error:", err);
        alert(err.message);
    }
}

/**
 * Show the Add Habit modal (remove hidden class).
 */
function showAddHabitModal() {
    document.getElementById('addHabitModal').classList.remove('hidden');
}

/**
 * Close the Add Habit modal (add hidden class).
 */
function closeAddHabitModal() {
    document.getElementById('addHabitModal').classList.add('hidden');
}

/**
 * Add a new habit via AJAX POST to /Habit/AddHabit.
 * On success, dynamically append a new row to the habit table with ✘ buttons for each day.
 */
async function addHabit(e) {
    e.preventDefault();

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    const category = document.getElementById('category').value;
    const habitName = document.getElementById('habitName').value;
    const habitColor = document.getElementById('habitColor').value;

    try {
        const res = await fetch('/Habit/AddHabit', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token
            },
            body: new URLSearchParams({ category, habitName, habitColor })
        });

        if (!res.ok) throw new Error("Failed to add habit");
        const newHabit = await res.json();

        // Build new row with habit name + daily ✘ buttons
        const table = document.querySelector('#habitTable tbody');
        const row = document.createElement('tr');
        const daysInMonth = new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate();

        row.innerHTML = `
            <td class="border px-4 py-2 font-semibold" style="color:${newHabit.color}">${newHabit.name}</td>
            ${Array.from({ length: daysInMonth }, (_, i) => `
                <td class="border px-2 py-1 text-center">
                    <button type="button"
                            class="px-2 py-1 rounded bg-red-500 text-white"
                            data-habit="${newHabit.id}"
                            data-date="${new Date().getFullYear()}-${(new Date().getMonth() + 1).toString().padStart(2, '0')}-${(i + 1).toString().padStart(2, '0')}"
                            data-status="true">✘</button>
                </td>`).join('')}
        `;
        table.appendChild(row);

        closeAddHabitModal();
    } catch (err) {
        console.error("Add habit error:", err);
        alert(err.message);
    }
}

/**
 * Load suggestions when category changes.
 * Fetches /Habit/Suggestions?category=... and renders clickable list items.
 */
document.getElementById('category').addEventListener('change', async function () {
    const category = this.value;
    const suggestionBox = document.getElementById('habitSuggestions');

    try {
        const res = await fetch(`/Habit/Suggestions?category=${encodeURIComponent(category)}`);
        if (!res.ok) throw new Error("Failed to load suggestions");

        const suggestions = await res.json();
        suggestionBox.innerHTML = suggestions.map(s => `
            <li class="cursor-pointer hover:bg-gray-100 p-1">${s}</li>
        `).join('');

        // Allow clicking a suggestion to auto-fill habitName
        suggestionBox.querySelectorAll('li').forEach(li => {
            li.addEventListener('click', () => {
                document.getElementById('habitName').value = li.textContent;
            });
        });
    } catch (err) {
        console.error("Suggestion error:", err);
    }
});

/**
 * Load categories dynamically when modal opens.
 * Fetches /Habit/Categories and populates <select>.
 */
async function loadCategories() {
    try {
        const res = await fetch('/Habit/Categories');
        if (!res.ok) throw new Error("Failed to load categories");

        const categories = await res.json();
        const categorySelect = document.getElementById('category');
        categorySelect.innerHTML = categories.map(c =>
            `<option value="${c.id}">${c.name}</option>`
        ).join('');
    } catch (err) {
        console.error("Category load error:", err);
    }
}

/**
 * Fetch suggestions as user types in habitName.
 * Uses /Habit/Suggestions?categoryId=...&query=...
 */
document.getElementById('habitName').addEventListener('input', async function () {
    const query = this.value;
    const categoryId = document.getElementById('category').value;
    if (!categoryId) return;

    try {
        const res = await fetch(`/Habit/Suggestions?categoryId=${categoryId}&query=${encodeURIComponent(query)}`);
        if (!res.ok) throw new Error("Failed to load suggestions");

        const suggestions = await res.json();
        const datalist = document.getElementById('habitSuggestions');
        datalist.innerHTML = suggestions.map(s => `<option value="${s}">`).join('');
    } catch (err) {
        console.error("Suggestion error:", err);
    }
});

/**
 * Event bindings:
 * - Add Habit button opens modal + loads categories
 * - Close button hides modal
 * - Form submit triggers addHabit()
 * - Any click on a habit button triggers toggleHabit()
 */
document.getElementById('addHabitBtn').addEventListener('click', () => {
    loadCategories();
    showAddHabitModal();
});
document.getElementById('closeModal').addEventListener('click', closeAddHabitModal);
document.getElementById('addHabitForm').addEventListener('submit', addHabit);
document.addEventListener('click', (e) => {
    const btn = e.target.closest('button[data-habit]');
    if (btn) toggleHabit(btn);
});

/**
 * Users theme selection and css implementation
 */
console.log("site.js loaded");

document.addEventListener("DOMContentLoaded", function () {
    debugger;
    console.log("DOM ready");

    const toggleBtn = document.getElementById("themeToggle");
    console.log("toggleBtn:", toggleBtn);

    const themeLink = document.querySelector("link[href*='site']");
    console.log("themeLink:", themeLink?.href);

    const themeHidden = document.getElementById("themePrefHidden"); // hidden field from _Layout.cshtml
    console.log("themeHidden initial value:", themeHidden?.value);

    const themeIcon = document.getElementById("themeIcon"); // sun/moon icon span

    if (!toggleBtn) {
        console.error("themeToggle button not found in DOM");
        return;
    }

    toggleBtn.addEventListener("click", function () {
        console.log("Theme toggle clicked");

        // ✅ Read current theme from hidden field instead of sessionStorage
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

        // ✅ Persist to backend
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
 
/********************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************
 *           Dashboard Visualizations (Chart.js)
 *******************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************/
 
/**
 * Weekly Completion Rate (Doughnut chart).
 * Shows percentage of habits completed vs missed for the current week.
 * Input: weeklyRate (number between 0–100).
 */
function initWeeklyRateChart(weeklyRate) {
    new Chart(document.getElementById('weeklyRateChart'), {
        type: 'doughnut',
        data: {
            labels: ['Completed', 'Missed'],
            datasets: [{
                data: [weeklyRate, 100 - weeklyRate],
                backgroundColor: ['#10b981', '#ef4444'] // pastel green/red
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: false }, // hide legend for cleaner look
                tooltip: { enabled: true }  // show tooltips on hover
            }
        }
    });
}

/**
 * Completion Trend (Line chart).
 * Plots daily/weekly trend of completed vs missed habits.
 * Input: arrays trendLabels (dates), completedData (counts), missedData (counts).
 */
function initTrendChart(trendLabels, completedData, missedData) {
    new Chart(document.getElementById('habitTrendChart'), {
        type: 'line',
        data: {
            labels: trendLabels,
            datasets: [
                {
                    label: 'Completed',
                    data: completedData,
                    borderColor: '#10b981', // pastel green line
                    backgroundColor: 'rgba(16,185,129,0.2)', // translucent fill
                    fill: true
                },
                {
                    label: 'Missed',
                    data: missedData,
                    borderColor: '#ef4444', // pastel red line
                    backgroundColor: 'rgba(239,68,68,0.2)',
                    fill: true
                }
            ]
        },
        options: {
            responsive: true,
            scales: { y: { beginAtZero: true } } // y-axis starts at 0
        }
    });
}

/**
 * Category Distribution (Pie chart).
 * Shows how habits are distributed across categories (e.g., Health, Work, Study).
 * Input: arrays categoryLabels (names), categoryValues (counts).
 */
function initCategoryChart(categoryLabels, categoryValues) {
    const categoryColors = ["#3b82f6", "#10b981", "#f59e0b", "#ef4444", "#6366f1"]; // pastel palette
    new Chart(document.getElementById('habitCategoryChart'), {
        type: 'pie',
        data: {
            labels: categoryLabels,
            datasets: [{
                data: categoryValues,
                backgroundColor: categoryColors.slice(0, categoryLabels.length)
            }]
        },
        options: { responsive: true }
    });
}

/**
 * Habit Streaks (Bar chart).
 * Displays streak length (consecutive days completed) for each habit.
 * Input: arrays streakLabels (habit names), streakValues (streak counts).
 */
function initStreakChart(streakLabels, streakValues) {
    new Chart(document.getElementById('habitStreakChart'), {
        type: 'bar',
        data: {
            labels: streakLabels,
            datasets: [{
                label: 'Streak Length',
                data: streakValues,
                backgroundColor: '#6366f1' // pastel indigo
            }]
        },
        options: {
            responsive: true,
            scales: { y: { beginAtZero: true } }
        }
    });
}

/**
 * Top 5 Consistent Habits (Horizontal bar chart).
 * Shows completion % for the most consistent habits.
 * Input: arrays topHabitLabels (habit names), topHabitValues (percentages).
 */
function initTopHabitsChart(topHabitLabels, topHabitValues) {
    new Chart(document.getElementById('topHabitsChart'), {
        type: 'bar',
        data: {
            labels: topHabitLabels,
            datasets: [{
                label: 'Completion %',
                data: topHabitValues,
                backgroundColor: '#10b981' // pastel green
            }]
        },
        options: {
            indexAxis: 'y', // horizontal bars
            responsive: true,
            scales: { x: { beginAtZero: true, max: 100 } } // x-axis capped at 100%
        }
    });
}