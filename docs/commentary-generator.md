# CommentaryGenerator

> Provides random commentary for simulating productivity and distracting from actual progress.

## Overview

`CommentaryGenerator` is a utility for generating random, often unhelpful, commentary. It is used by progress simulators and spinners to create the illusion of activity.

## Usage

```csharp
using ProcrastiN8.Common;

var comment = CommentaryGenerator.GetRandomCommentary();
Console.WriteLine($"Commentary: {comment}");
```

## API

### `GetRandomCommentary`

```csharp
public static string GetRandomCommentary()
```

- Returns a random commentary string.

## Remarks

- Commentary is designed for maximum distraction and minimum value.
- Used by all core simulation features.
