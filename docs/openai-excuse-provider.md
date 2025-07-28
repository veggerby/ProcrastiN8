# OpenAI Excuse Provider

The `OpenAIExcuseProvider` is a feature of ProcrastiN8 that leverages OpenAI's ChatGPT API to generate creative and humorous excuses for procrastination.

## Features

- Fetches excuses dynamically from OpenAI's GPT-4 model.
- Provides topical and contextually relevant excuses.
- Fully integrated with the ProcrastiN8 ecosystem.

## Usage

```csharp
using ProcrastiN8.Common;

var excuseProvider = new OpenAIExcuseProvider("your-api-key");
var excuse = await excuseProvider.GetExcuseAsync();
Console.WriteLine(excuse);
```

## Requirements

- An OpenAI API key.
- Internet connectivity.

## Remarks

This provider is ideal for developers who want to add a touch of AI-powered humor to their procrastination toolkit.
