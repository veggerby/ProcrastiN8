# 🐢 Contributing to ProcrastiN8

First off — **thank you** for considering a contribution to **ProcrastiN8**.
This project exists because of people like you who believe in building *better* systems — not just *bigger* ones.

ProcrastiN8 is still in its early stages (deliberately so), and **every contribution matters** — whether it's a bug fix, feature idea, doc update, or philosophical debate about laziness-as-a-service™.

Let’s build the most overengineered productivity avoidance toolkit the world never needed.

---

## 🚀 How to Contribute

There are many ways you can help:

- **Report bugs**: Found something strange or broken? Open an issue.
- **Suggest improvements**: Ideas for features, extensions, better defaults — always welcome.
- **Improve documentation**: Good docs make the project more accessible to everyone.
- **Submit code changes**: Fix a bug, add a feature, or improve internal structure.

---

## 📋 Contribution Guidelines

Please follow these basic guidelines to keep everything smooth:

1. **Open an Issue First**
   If you're planning a larger change, open an issue first to discuss it. It helps avoid duplicated work or big surprises.

2. **Small Pull Requests**
   Try to keep PRs focused and easy to review. Smaller changes get merged faster.

3. **Write Tests**
   If you're fixing a bug or adding a new feature, please include or update tests to cover it.

4. **Match the Code Style**
   Keep code clean and consistent.
   - Use full curly braces `{}` for conditionals and loops.
   - Use async/await naturally where needed.
   - Prefer immutability for models and value objects when possible.

5. **Explain Your Changes**
   In PRs, explain the "why" as well as the "what". A few clear sentences are enough.

---

## 🛠 Local Setup

- Clone the repository.
- Build the solution (`ProcrastiN8.sln`) using .NET 10 SDK (recommended) or later.
- Run the tests (`ProcrastiN8.Tests`) to make sure everything passes before you push.

```bash
dotnet build
dotnet test
```

---

## 🧩 Project Structure (Overview)

| Folder | Purpose |
|:-------|:--------|
| `/src/ProcrastiN8` | Core library: quantum procrastination, lazy tasks, metrics, services |
| `/test/ProcrastiN8.Tests` | Unit and integration tests |
| `/docs` | Documentation |

---

## 🛡️ Code of Conduct

Be kind.
ProcrastiN8 welcomes contributors from all backgrounds and skill levels. No toxicity, no gatekeeping. We’re here to build together.

---

## 📢 Final Thoughts

ProcrastiN8 isn't trying to reinvent productivity tools.
It’s about making creative procrastination accessible to every .NET developer, with a bit of quantum weirdness and humor.

Thank you for helping make that happen.

— The ProcrastiN8 Team
