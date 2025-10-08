// File: wwwroot/js/site.js
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// *****************************
//  Dashboard
// *******************************
document.addEventListener("DOMContentLoaded", function () {
  // User authentication simulation (replace with real data if needed)
  const users = [
    {
      id: 1,
      name: "Mark Johnson",
      email: "mark@example.com",
      profileImage: "https://randomuser.me/api/portraits/men/32.jpg",
      hasImage: true,
    },
    {
      id: 2,
      name: "Sarah Wilson",
      email: "sarah@example.com",
      profileImage: null,
      hasImage: false,
    },
    {
      id: 3,
      name: "Alex Chen",
      email: "alex@example.com",
      profileImage: "https://randomuser.me/api/portraits/men/67.jpg",
      hasImage: true,
    },
  ];

  function getUserInitials(name) {
    if (!name) return "";
    return name
      .split(" ")
      .map((word) => word[0])
      .join("")
      .toUpperCase();
  }

  function displayUserProfile(user) {
    const userGreeting = document.getElementById("userGreeting");
    if (userGreeting) {
      userGreeting.textContent = `Hello, ${user.name.split(" ")[0]}!`;
    }

    const userProfileDisplay = document.getElementById("userProfileDisplay");
    if (!userProfileDisplay) return;

    if (user.hasImage && user.profileImage) {
      userProfileDisplay.innerHTML = `<img src="${user.profileImage}" class="profile-image" alt="${user.name}">`;
    } else {
      const initials = getUserInitials(user.name);
      userProfileDisplay.innerHTML = `<div class="profile-icon">${initials}</div>`;
    }
  }

  const currentUser = users[0];
  displayUserProfile(currentUser);

  // Initialize dashboard chart only if canvas exists
  const canvas = document.getElementById("expensesChart");
  if (!canvas) {
    console.log("expensesChart canvas not found (safe to ignore on this page).");
    return;
  }

  const ctx = canvas.getContext("2d");
  const expensesChart = new Chart(ctx, {
    type: "doughnut",
    data: {
      labels: [
        "House",
        "Savings",
        "Transportation",
        "Groceries",
        "Shopping",
        "Entertainment",
      ],
      datasets: [
        {
          data: [41.35, 21.51, 15.47, 9.27, 7.35, 5.05],
          backgroundColor: [
            "#6366f1",
            "#ef4444",
            "#06b6d4",
            "#10b981",
            "#8b5cf6",
            "#f59e0b",
          ],
          borderWidth: 0,
          cutout: "60%",
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: { legend: { display: false } },
    },
  });

  // Dashboard data and update logic
  const timePeriodsData = {
    "This month": {
      balance: { value: "$5,502.45", change: "+ 12.5%", positive: true },
      incomes: { value: "$9,450.00", change: "+ 27%", positive: true },
      expenses: { value: "$3,945.55", change: "- 15%", positive: false },
      chartData: [41.35, 21.51, 15.47, 9.27, 7.35, 5.05],
      categories: [
        { name: "House", percentage: "41.35%", color: "#6366f1" },
        { name: "Savings", percentage: "21.51%", color: "#ef4444" },
        { name: "Transportation", percentage: "15.47%", color: "#06b6d4" },
        { name: "Groceries", percentage: "9.27%", color: "#10b981" },
        { name: "Shopping", percentage: "7.35%", color: "#8b5cf6" },
        { name: "Entertainment", percentage: "5.05%", color: "#f59e0b" },
      ],
    },
  };

  function updateDashboard(period) {
    const data = timePeriodsData[period];
    if (!data) return;

    const metricCards = document.querySelectorAll(".metric-card");
    if (metricCards.length >= 3) {
      const balanceValue = metricCards[0].querySelector(".metric-value");
      const balanceChange = metricCards[0].querySelector(".metric-change");
      const incomesValue = metricCards[1].querySelector(".metric-value");
      const incomesChange = metricCards[1].querySelector(".metric-change");
      const expensesValue = metricCards[2].querySelector(".metric-value");
      const expensesChange = metricCards[2].querySelector(".metric-change");

      if (balanceValue) balanceValue.textContent = data.balance.value;
      if (balanceChange) {
        balanceChange.textContent = data.balance.change;
        balanceChange.className =
          "metric-change " + (data.balance.positive ? "positive" : "negative");
      }

      if (incomesValue) incomesValue.textContent = data.incomes.value;
      if (incomesChange) {
        incomesChange.textContent = data.incomes.change;
        incomesChange.className =
          "metric-change " + (data.incomes.positive ? "positive" : "negative");
      }

      if (expensesValue) expensesValue.textContent = data.expenses.value;
      if (expensesChange) {
        expensesChange.textContent = data.expenses.change;
        expensesChange.className =
          "metric-change " + (data.expenses.positive ? "positive" : "negative");
      }
    }

    if (expensesChart && data.chartData) {
      expensesChart.data.datasets[0].data = data.chartData;
      expensesChart.update("active");
    }

    const categoryItems = document.querySelectorAll(".category-item");
    data.categories &&
      data.categories.forEach((category, index) => {
        const item = categoryItems[index];
        if (!item) return;
        const nameEl = item.querySelector(".category-name");
        const percEl = item.querySelector(".category-percentage");
        const colorEl = item.querySelector(".category-color");
        if (nameEl) nameEl.textContent = category.name;
        if (percEl) percEl.textContent = category.percentage;
        if (colorEl) colorEl.style.backgroundColor = category.color;
      });
  }

  document.querySelectorAll(".time-filter").forEach((filter) => {
    filter.addEventListener("click", function () {
      document
        .querySelectorAll(".time-filter")
        .forEach((f) => f.classList.remove("active"));
      this.classList.add("active");
      updateDashboard(this.textContent);
    });
  });

  updateDashboard("This month");
});

// *****************************
//  Add expenses
// *******************************
const expenseForm = document.getElementById("expenseForm");
if (expenseForm) {
  document.querySelectorAll(".category-option").forEach((option) => {
    option.addEventListener("click", function () {
      document
        .querySelectorAll(".category-option")
        .forEach((opt) => opt.classList.remove("selected"));
      this.classList.add("selected");
      document.getElementById("selectedCategory").value = this.dataset.category;
    });
  });

  const recurringCheckbox = document.getElementById("recurring");
  if (recurringCheckbox) {
    recurringCheckbox.addEventListener("change", function () {
      const recurringOptions = document.getElementById("recurringOptions");
      if (recurringOptions)
        recurringOptions.style.display = this.checked ? "block" : "none";
    });
  }

  expenseForm.addEventListener("submit", function (e) {
    e.preventDefault();
    const formData = {
      category: document.getElementById("selectedCategory").value,
      description: document.getElementById("description").value,
      amount: parseFloat(document.getElementById("amount").value),
      date: document.getElementById("date").value,
    };
    if (!formData.category || !formData.description || !formData.amount || !formData.date) {
      alert("Please fill in all required fields and select a category.");
      return;
    }
    console.log("Expense data:", formData);
    alert("Expense added successfully!");
    window.location.href = "dashboard.html";
  });
}

// *****************************
//  Add income
// *******************************
const incomeForm = document.getElementById("incomeForm");
if (incomeForm) {
  const dateInput = document.getElementById("date");
  if (dateInput) dateInput.valueAsDate = new Date();

  const recurringCheckbox = document.getElementById("recurring");
  if (recurringCheckbox) {
    recurringCheckbox.addEventListener("change", function () {
      const recurringOptions = document.getElementById("recurringOptions");
      if (recurringOptions)
        recurringOptions.style.display = this.checked ? "block" : "none";
    });
  }

  incomeForm.addEventListener("submit", function (e) {
    e.preventDefault();
    const formData = {
      description: document.getElementById("description").value,
      category: document.getElementById("category").value,
      amount: parseFloat(document.getElementById("amount").value),
      date: document.getElementById("date").value,
    };
    if (!formData.description || !formData.category || !formData.amount || !formData.date) {
      alert("Please fill in all required fields.");
      return;
    }
    console.log("Income data:", formData);
    alert("Income added successfully!");
    window.location.href = "dashboard.html";
  });
}


// File: wwwroot/js/site.js

// Function to handle file downloads
window.downloadFile = function (base64Data, contentType, fileName) {
    try {
        const link = document.createElement('a');
        link.download = fileName;
        link.href = `data:${contentType};base64,${base64Data}`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        return true;
    } catch (error) {
        console.error('Download error:', error);
        return false;
    }
};

// Alternative download function using Blob
window.downloadCsv = function (content, fileName) {
    try {
        const blob = new Blob([content], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        link.setAttribute('href', url);
        link.setAttribute('download', fileName);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
        return true;
    } catch (error) {
        console.error('CSV download error:', error);
        return false;
    }
};