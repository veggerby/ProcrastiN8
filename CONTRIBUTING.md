# Contributing to Shardis

First off — thank you for considering contributing to Shardis!
This project exists because of people like you who want to build better systems, not just bigger ones.

Shardis is still early in its journey, and every contribution — big or small — makes a real difference.

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
- Build the solution (`Shardis.sln`) using .NET 8 or later.
- Run the tests (`Shardis.Tests`) to make sure everything passes before you push.

```bash
dotnet build
dotnet test
```

---

## 🧩 Project Structure (Overview)

| Folder | Purpose |
|:-------|:--------|
| `/Shardis` | Core library: routing, models, persistence abstractions |
| `/Shardis.Tests` | Unit and integration tests |
| `/SampleApp` | Example usage (coming soon) |

---

## 🛡️ Code of Conduct

Be kind.
Shardis welcomes contributors from all backgrounds and skill levels. No toxicity, no gatekeeping. We’re here to build together.

---

## 📢 Final Thoughts

Shardis isn't trying to reinvent databases.
It’s about making scalable design accessible to every .NET developer, without the usual pain.

Thank you for helping make that happen.

— The Shardis Team
