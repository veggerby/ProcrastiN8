# Cross-Distribution Package Manager

A devcontainer feature that installs system packages with automatic distribution detection, supporting both apt (Debian/Ubuntu) and apk (Alpine Linux) package managers.

## Features

- 🔍 **Auto-detection**: Automatically detects whether the container uses apt or apk
- 📦 **Distro-specific packages**: Specify different package names for each distribution
- 🧹 **Clean installation**: Cleans up package manager caches to minimize image size
- ⚡ **Fast**: No unnecessary updates or cache rebuilds

## Usage

### Basic Example

```json
{
    "features": {
        "./features/cross-distro-packages": {
            "apt": "build-essential python3-dev",
            "apk": "build-base python3-dev"
        }
    }
}
```

### Real-World Examples

**Node.js Build Tools:**

```json
{
    "features": {
        "./features/cross-distro-packages": {
            "apt": "build-essential",
            "apk": "build-base"
        }
    }
}
```

**Python Development:**

```json
{
    "features": {
        "./features/cross-distro-packages": {
            "apt": "build-essential python3-dev",
            "apk": "build-base python3-dev"
        }
    }
}
```

**Redis CLI Tools:**

```json
{
    "features": {
        "./features/cross-distro-packages": {
            "apt": "redis-tools",
            "apk": "redis"
        }
    }
}
```

**.NET Dependencies:**

```json
{
    "features": {
        "./features/cross-distro-packages": {
            "apt": "xdg-utils pass sshpass build-essential netcat-traditional iputils-ping dnsutils git-lfs sqlite3",
            "apk": "xdg-utils pass sshpass build-base netcat-openbsd iputils bind-tools git-lfs sqlite"
        }
    }
}
```

## Options

| Option | Type   | Default | Description                                                                   |
| ------ | ------ | ------- | ----------------------------------------------------------------------------- |
| `apt`  | string | `""`    | Space-separated list of packages for apt-based distributions (Debian, Ubuntu) |
| `apk`  | string | `""`    | Space-separated list of packages for apk-based distributions (Alpine Linux)   |

## Package Name Differences

Common packages that have different names across distributions:

| Debian/Ubuntu (apt)  | Alpine (apk)     | Purpose                       |
| -------------------- | ---------------- | ----------------------------- |
| `build-essential`    | `build-base`     | C/C++ compiler toolchain      |
| `dnsutils`           | `bind-tools`     | DNS utilities (dig, nslookup) |
| `netcat-traditional` | `netcat-openbsd` | Network utility               |
| `sqlite3`            | `sqlite`         | SQLite database               |
| `redis-tools`        | `redis`          | Redis CLI tools               |

## How It Works

1. Checks for `apk` command (Alpine Linux)
2. Falls back to `apt-get` (Debian/Ubuntu)
3. **Deduplicates** the package list (if multiple overlays specify the same packages)
4. Installs the appropriate package list
5. Cleans up package manager caches

## Multiple Overlays

When using multiple overlays that reference this feature, packages are **automatically deduplicated** at two levels:

1. **Composer-level merge**: The composition engine merges all cross-distro-packages declarations before writing devcontainer.json
2. **Runtime deduplication**: The install script also deduplicates packages as a safety measure

**Example:**

```json
// nodejs overlay adds:
"./features/cross-distro-packages": {
  "apt": "build-essential"
}

// python overlay adds:
"./features/cross-distro-packages": {
  "apt": "build-essential python3-dev"
}

// Final devcontainer.json (after composition):
"./features/cross-distro-packages": {
  "apt": "build-essential python3-dev",  // deduplicated!
  "apk": "build-base python3-dev"
}
```

You don't need to worry about conflicts when combining overlays - the system handles it automatically.

## Notes

- Runs as root during container build
- Uses `--no-install-recommends` (apt) and `--no-cache` (apk) to minimize image size
- Automatically runs `apt-get update` before installing (apt only)
- Cleans up `/var/lib/apt/lists/*` after installation (apt only)

## Contributing

To add support for additional package managers (e.g., yum, dnf, pacman), update the detection logic in `install.sh`.
