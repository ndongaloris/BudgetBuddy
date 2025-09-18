# 💰 BudgetBuddy

**BudgetBuddy** is a personal finance management web application built with ASP.NET Core MVC. It empowers users to track bank accounts, credit cards, investments, and digital wallets—all in one place.

---

## 🚀 Features

- 🔐 Secure user authentication (local + external providers like Google/GitHub)
- 🧾 Account dashboard with real-time balance summaries
- 💳 Support for multiple account types: bank, credit card, investment, wallet, cash
- 📊 Investment tracking with performance metrics
- 🧠 Profile initials and image support for personalized UI
- 📁 Razor views with shared layout and runtime compilation
- 🌐 Responsive design for desktop and mobile

---

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core MVC, Identity
- **Frontend:** Razor Views, Bootstrap, custom CSS (`site.css`)
- **Database:** Entity Framework Core (SQL Server or SQLite)
- **Authentication:** ASP.NET Core Identity + External OAuth providers

---

## 📂 Folder Structure

Financy/ ├── Controllers/ │ └── AccountController.cs ├── Models/ │ └── User.cs, ViewModels/ ├── Views/ │ ├── Account/ │ │ └── Login.cshtml, Register.cshtml │ ├── Shared/ │ │ └── _Layout.cshtml │ └── Transactions/ │ └── Index.cshtml ├── wwwroot/ │ └── css/ │ └── site.css ├── Program.cs └── appsettings.json

---

## 🧪 Getting Started

1. **Clone the repo**
   ```bash
   git clone https://github.com/yourusername/BudgetBuddy.git
   cd BudgetBuddy

2. **Install dependencies**
   ```bash
    dotnet restore

3. **Run the app with Hot Reload**
   ```bash
    dotnet watch run

4. **Access locally**
   ```bash
    http://localhost:5291

## 👥 Team Members

| Name              | Role                | GitHub Username      |
|-------------------|---------------------|----------------------|
| Loris J. Ndonga   | Software Developer  | @ndongaloris         |
| Alexander Cyril   | Database Architect  | @tobefilled          |
