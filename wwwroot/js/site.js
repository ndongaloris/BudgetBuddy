// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// *****************************
//  Dashboard
// *******************************
// Wrap everything in DOMContentLoaded so missing elements won't break initialization
document.addEventListener("DOMContentLoaded", function () {
  // User authentication simulation (you can replace with real data)
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

  // Only update profile display if the element exists (nav might be in layout)
  function displayUserProfile(user) {
    const userGreeting = document.getElementById("userGreeting");
    if (userGreeting) {
      userGreeting.textContent = `Hello, ${user.name.split(" ")[0]}!`;
    }

    const userProfileDisplay = document.getElementById("userProfileDisplay");
    if (!userProfileDisplay) return; // nav/profile area likely lives in layout now

    if (user.hasImage && user.profileImage) {
      userProfileDisplay.innerHTML = `<img src="${user.profileImage}" class="profile-image" alt="${user.name}">`;
    } else {
      const initials = getUserInitials(user.name);
      userProfileDisplay.innerHTML = `<div class="profile-icon">${initials}</div>`;
    }
  }

  // Pick the current user for demo
  const currentUser = users[0];
  displayUserProfile(currentUser);

  // Initialize the donut chart safely
  const canvas = document.getElementById("expensesChart");
  if (!canvas) {
    console.error("expensesChart canvas not found.");
    return;
  }

  const ctx = canvas.getContext("2d");

  // Build the Chart instance
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

  // Data sets for different time periods
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
    "Last month": {
      /* ... */ chartData: [38.2, 24.15, 18.3, 11.45, 5.2, 2.7],
      balance: { value: "$4,892.30", change: "+ 8.2%", positive: true },
      incomes: { value: "$8,750.00", change: "+ 15%", positive: true },
      expenses: { value: "$4,257.70", change: "+ 5%", positive: false },
      categories: [
        { name: "House", percentage: "38.20%", color: "#6366f1" },
        { name: "Savings", percentage: "24.15%", color: "#ef4444" },
        { name: "Transportation", percentage: "18.30%", color: "#06b6d4" },
        { name: "Groceries", percentage: "11.45%", color: "#10b981" },
        { name: "Shopping", percentage: "5.20%", color: "#8b5cf6" },
        { name: "Entertainment", percentage: "2.70%", color: "#f59e0b" },
      ],
    },
    "This year": {
      /* ... */ chartData: [42.8, 19.6, 16.2, 8.9, 8.5, 4.0],
      balance: { value: "$48,250.80", change: "+ 35.7%", positive: true },
      incomes: { value: "$95,400.00", change: "+ 42%", positive: true },
      expenses: { value: "$47,149.20", change: "+ 18%", positive: false },
      categories: [
        { name: "House", percentage: "42.80%", color: "#6366f1" },
        { name: "Savings", percentage: "19.60%", color: "#ef4444" },
        { name: "Transportation", percentage: "16.20%", color: "#06b6d4" },
        { name: "Groceries", percentage: "8.90%", color: "#10b981" },
        { name: "Shopping", percentage: "8.50%", color: "#8b5cf6" },
        { name: "Entertainment", percentage: "4.00%", color: "#f59e0b" },
      ],
    },
    "Last 12 months": {
      /* ... */ chartData: [40.5, 22.3, 14.8, 10.2, 7.4, 4.8],
      balance: { value: "$52,180.95", change: "+ 28.4%", positive: true },
      incomes: { value: "$118,500.00", change: "+ 38%", positive: true },
      expenses: { value: "$66,319.05", change: "+ 22%", positive: false },
      categories: [
        { name: "House", percentage: "40.50%", color: "#6366f1" },
        { name: "Savings", percentage: "22.30%", color: "#ef4444" },
        { name: "Transportation", percentage: "14.80%", color: "#06b6d4" },
        { name: "Groceries", percentage: "10.20%", color: "#10b981" },
        { name: "Shopping", percentage: "7.40%", color: "#8b5cf6" },
        { name: "Entertainment", percentage: "4.80%", color: "#f59e0b" },
      ],
    },
  };

  // More robust selector for metric cards (works even if layout nesting changes)
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

    // Update chart
    if (expensesChart && data.chartData) {
      expensesChart.data.datasets[0].data = data.chartData;
      expensesChart.update("active");
    }

    // Update categories list
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

  // Wire up time filters
  document.querySelectorAll(".time-filter").forEach((filter) => {
    filter.addEventListener("click", function () {
      document
        .querySelectorAll(".time-filter")
        .forEach((f) => f.classList.remove("active"));
      this.classList.add("active");
      updateDashboard(this.textContent);
    });
  });

  // initialize with default period
  updateDashboard("This month");
});

// *****************************
//  Add expenses
// *******************************
// Category selection
document.querySelectorAll(".category-option").forEach((option) => {
  option.addEventListener("click", function () {
    // Remove selected class from all options
    document
      .querySelectorAll(".category-option")
      .forEach((opt) => opt.classList.remove("selected"));

    // Add selected class to clicked option
    this.classList.add("selected");

    // Set hidden input value
    document.getElementById("selectedCategory").value = this.dataset.category;
  });
});

// Toggle recurring options
document.getElementById("recurring").addEventListener("change", function () {
  const recurringOptions = document.getElementById("recurringOptions");
  if (this.checked) {
    recurringOptions.style.display = "block";
  } else {
    recurringOptions.style.display = "none";
  }
});

// Form submission
document.getElementById("expenseForm").addEventListener("submit", function (e) {
  e.preventDefault();

  // Get form data
  const formData = {
    category: document.getElementById("selectedCategory").value,
    description: document.getElementById("description").value,
    amount: parseFloat(document.getElementById("amount").value),
    date: document.getElementById("date").value,
    account: document.getElementById("account").value,
    paymentMethod: document.getElementById("paymentMethod").value,
    vendor: document.getElementById("vendor").value,
    notes: document.getElementById("notes").value,
    recurring: document.getElementById("recurring").checked,
    frequency: document.getElementById("frequency").value,
    endDate: document.getElementById("endDate").value,
    businessExpense: document.getElementById("businessExpense").checked,
  };

  // Validate required fields
  if (
    !formData.category ||
    !formData.description ||
    !formData.amount ||
    !formData.date
  ) {
    alert("Please fill in all required fields and select a category.");
    return;
  }

  // Simulate saving (in a real app, this would send to a server)
  console.log("Expense data:", formData);

  // Show success message and redirect
  alert("Expense added successfully!");
  window.location.href = "dashboard.html";
});

// *****************************
//  Add income
// *******************************
// Set today's date as default
document.getElementById("date").valueAsDate = new Date();

// Toggle recurring options
document.getElementById("recurring").addEventListener("change", function () {
  const recurringOptions = document.getElementById("recurringOptions");
  if (this.checked) {
    recurringOptions.style.display = "block";
  } else {
    recurringOptions.style.display = "none";
  }
});

// Form submission
document.getElementById("incomeForm").addEventListener("submit", function (e) {
  e.preventDefault();

  // Get form data
  const formData = {
    description: document.getElementById("description").value,
    category: document.getElementById("category").value,
    amount: parseFloat(document.getElementById("amount").value),
    date: document.getElementById("date").value,
    account: document.getElementById("account").value,
    paymentMethod: document.getElementById("paymentMethod").value,
    notes: document.getElementById("notes").value,
    recurring: document.getElementById("recurring").checked,
    frequency: document.getElementById("frequency").value,
    endDate: document.getElementById("endDate").value,
  };

  // Validate required fields
  if (
    !formData.description ||
    !formData.category ||
    !formData.amount ||
    !formData.date
  ) {
    alert("Please fill in all required fields.");
    return;
  }

  // Simulate saving (in a real app, this would send to a server)
  console.log("Income data:", formData);

  // Show success message and redirect
  alert("Income added successfully!");
  window.location.href = "dashboard.html";
});
