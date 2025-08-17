# FractalDataWorks.Net

[![NuGet](https://img.shields.io/nuget/v/FractalDataWorks.Net.svg)](https://www.nuget.org/packages/FractalDataWorks.Net/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

A foundational .NET library providing core abstractions, interfaces, and utilities for building robust, scalable applications using the FractalDataWorks architecture patterns.

## Overview

FractalDataWorks.Net is the base library for the FractalDataWorks ecosystem, providing:

- **Core Abstractions**: Foundational interfaces and base classes for services, configurations, and results
- **Configuration Management**: Flexible configuration system with providers, sources, and change notifications
- **Connection Management**: Abstractions for internal and external connections with factory patterns
- **Validation Framework**: Comprehensive validation interfaces with severity levels and error handling
- **Result Patterns**: Generic result types for operation outcomes with success/failure patterns
- **Message System**: Structured messaging with severity levels for diagnostics and logging
- **Source Generation Utilities**: Tools for incremental code generation with hash-based change detection

## Installation

### NuGet Package Manager
```powershell
Install-Package FractalDataWorks.Net
```

### .NET CLI
```bash
dotnet add package FractalDataWorks.Net
```

### PackageReference
```xml
<PackageReference Include="FractalDataWorks.Net" Version="*.*.*" />
```

## Features

### Core Types

#### Fractal Unit Type
A unit type for operations that don't return a value, similar to `void` but usable as a type parameter:

```csharp
public struct Fractal : IEquatable<Fractal>
{
    public static readonly Fractal Value;
}

// Usage in generic results
IFdwResult<Fractal> DeleteOperation() => FdwResult.Success(Fractal.Value);
```

#### Result Patterns
Comprehensive result types for operation outcomes:

```csharp
// Generic result interface
public interface IGenericResult
{
    bool IsSuccess { get; }
    string Message { get; }
}

// FractalDataWorks-specific result
public interface IFdwResult : IGenericResult
{
    IEnumerable<IFdwMessage> Messages { get; }
}

// Generic typed result
public interface IFdwResult<T> : IFdwResult
{
    T Value { get; }
}

// Result implementations
public class FdwResult : IFdwResult
{
    public static FdwResult Success(string message = null);
    public static FdwResult Failure(string message);
    public static FdwResult<T> Success<T>(T value, string message = null);
    public static FdwResult<T> Failure<T>(string message);
}

// Logging extensions
public static class FdwResultLoggingExtensions
{
    public static void LogResult(this IFdwResult result, ILogger logger);
}
```

### Configuration System

Flexible configuration management with multiple sources and providers:

```csharp
// Configuration base
public abstract class FdwConfigurationBase : IFdwConfiguration
{
    // Base implementation for configuration classes
}

// Configuration interfaces
public interface IFdwConfiguration
{
    string Name { get; }
    ConfigurationSourceType SourceType { get; }
}

public interface IFdwConfigurationProvider
{
    IFdwConfiguration GetConfiguration(string name);
    Task<IFdwConfiguration> GetConfigurationAsync(string name);
}

public interface IFdwConfigurationSource
{
    ConfigurationSourceType Type { get; }
    Task<IFdwConfiguration> LoadAsync();
}

public interface IConfigurationRegistry
{
    void Register(IFdwConfigurationSource source);
    IFdwConfiguration GetConfiguration(string name);
}

// Configuration enums
public enum ConfigurationSourceType
{
    File,
    Database,
    Environment,
    Memory,
    Remote
}

public enum ConfigurationChangeType
{
    Added,
    Modified,
    Removed,
    Reloaded
}

// Change notifications
public class ConfigurationSourceChangedEventArgs : EventArgs
{
    public ConfigurationChangeType ChangeType { get; }
    public IFdwConfigurationSource Source { get; }
}
```

### Service Architecture

Comprehensive service abstractions for dependency injection and factory patterns:

```csharp
// Core service interface
public interface IFdwService
{
    string ServiceName { get; }
    ServiceStatus Status { get; }
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}

// Service provider
public interface IFdwServiceProvider
{
    T GetService<T>() where T : IFdwService;
    IEnumerable<IFdwService> GetServices();
}

// Factory patterns
public interface IServiceFactory<T> where T : IFdwService
{
    T CreateService(IFdwConfiguration configuration);
}

public interface IToolFactory<T> where T : IFdwTool
{
    T CreateTool(string toolName);
}

// Tool abstraction
public interface IFdwTool
{
    string Name { get; }
    string Version { get; }
    Task<IFdwResult> ExecuteAsync(ICommand command);
}
```

### Connection Management

Abstractions for managing various types of connections:

```csharp
// Base connection interface
public interface IFdwConnection
{
    string ConnectionName { get; }
    bool IsConnected { get; }
    Task<bool> ConnectAsync();
    Task DisconnectAsync();
}

// External connections
public interface IExternalConnection : IFdwConnection
{
    Uri Endpoint { get; }
    TimeSpan Timeout { get; }
}

// Connection factories
public interface IConnectionFactory<T> where T : IFdwConnection
{
    T CreateConnection(string connectionString);
}

public interface IExternalConnectionFactory : IConnectionFactory<IExternalConnection>
{
    IExternalConnection CreateConnection(Uri endpoint, TimeSpan timeout);
}

// Data connections
public interface IDataConnection : IFdwConnection
{
    Task<IDataCommand> CreateCommandAsync(string commandText);
}

public interface IDataCommand : ICommand
{
    string CommandText { get; }
    Task<ICommandResult> ExecuteAsync();
}
```

### Command Pattern

Command abstraction for operations:

```csharp
public interface ICommand
{
    string Name { get; }
    IDictionary<string, object> Parameters { get; }
}

public interface ICommandResult
{
    bool Success { get; }
    object Data { get; }
    string ErrorMessage { get; }
}
```

### Validation Framework

Comprehensive validation system with multiple severity levels:

```csharp
// Validator interface
public interface IFdwValidator<T>
{
    IValidationResult Validate(T entity);
    Task<IValidationResult> ValidateAsync(T entity);
}

// Validation results
public interface IValidationResult
{
    bool IsValid { get; }
    IEnumerable<IValidationError> Errors { get; }
}

public interface IValidationError
{
    string PropertyName { get; }
    string ErrorMessage { get; }
    ValidationSeverity Severity { get; }
}

public enum ValidationSeverity
{
    Info,
    Warning,
    Error,
    Critical
}
```

### Message System

Structured messaging for diagnostics and logging:

```csharp
public interface IFdwMessage
{
    string Text { get; }
    MessageSeverity Severity { get; }
    DateTime Timestamp { get; }
    string Source { get; }
}

public enum MessageSeverity
{
    Verbose,
    Debug,
    Info,
    Warning,
    Error,
    Critical
}

// Notification priorities
public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Urgent,
    Critical
}
```

### Source Generation Utilities

Tools for incremental source generation with change detection:

```csharp
// Input tracking for incremental generation
public interface IInputInfo
{
    string InputHash { get; }
    void WriteToHash(TextWriter writer);
}

// Hash calculation utilities
public static class InputHashCalculator
{
    public static string CalculateHash(IInputInfo info);
    public static string CalculateInputHash<T>(T info) where T : IInputInfo;
    public static bool TrackedInputHasChanged<T>(T oldInfo, T newInfo) where T : IInputInfo;
    public static bool HasChanged(string? oldHash, string newHash);
}

// Usage in source generators
public class MyGeneratorModel : IInputInfo
{
    public string Name { get; set; }
    public string InputHash => InputHashCalculator.CalculateHash(this);
    
    public void WriteToHash(TextWriter writer)
    {
        writer.WriteLine(Name);
        // Write other properties for hash calculation
    }
}
```

## Usage Examples

### Result Pattern
```csharp
public IFdwResult<User> GetUser(int id)
{
    try
    {
        var user = repository.FindById(id);
        return user != null 
            ? FdwResult.Success(user)
            : FdwResult.Failure<User>("User not found");
    }
    catch (Exception ex)
    {
        return FdwResult.Failure<User>($"Error retrieving user: {ex.Message}");
    }
}

// Usage
var result = GetUser(123);
if (result.IsSuccess)
{
    Console.WriteLine($"Found user: {result.Value.Name}");
}
else
{
    Console.WriteLine($"Error: {result.Message}");
}
```

### Configuration System
```csharp
public class AppConfiguration : FdwConfigurationBase
{
    public string ConnectionString { get; set; }
    public int MaxRetries { get; set; }
}

public class ConfigurationService : IConfigurationRegistry
{
    private readonly Dictionary<string, IFdwConfiguration> _configurations = new();
    
    public void Register(IFdwConfigurationSource source)
    {
        var config = source.LoadAsync().Result;
        _configurations[config.Name] = config;
    }
    
    public IFdwConfiguration GetConfiguration(string name)
    {
        return _configurations.TryGetValue(name, out var config) 
            ? config 
            : null;
    }
}
```

### Service Implementation
```csharp
public class DataSyncService : IFdwService
{
    public string ServiceName => "DataSync";
    public ServiceStatus Status { get; private set; }
    
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        Status = ServiceStatus.Starting;
        // Initialize service
        Status = ServiceStatus.Running;
    }
    
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        Status = ServiceStatus.Stopping;
        // Cleanup
        Status = ServiceStatus.Stopped;
    }
}
```

### Validation Example
```csharp
public class UserValidator : IFdwValidator<User>
{
    public IValidationResult Validate(User user)
    {
        var errors = new List<IValidationError>();
        
        if (string.IsNullOrEmpty(user.Email))
        {
            errors.Add(new ValidationError
            {
                PropertyName = nameof(user.Email),
                ErrorMessage = "Email is required",
                Severity = ValidationSeverity.Error
            });
        }
        
        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }
}
```

### Incremental Generation
```csharp
public class GeneratorModel : IInputInfo
{
    public string TypeName { get; set; }
    public List<string> Properties { get; set; }
    
    public string InputHash => InputHashCalculator.CalculateHash(this);
    
    public void WriteToHash(TextWriter writer)
    {
        writer.WriteLine(TypeName);
        foreach (var prop in Properties)
        {
            writer.WriteLine(prop);
        }
    }
}

// In source generator
var oldModel = GetCachedModel();
var newModel = AnalyzeSource();

if (InputHashCalculator.TrackedInputHasChanged(oldModel, newModel))
{
    // Regenerate code only if input changed
    GenerateCode(newModel);
}
```

## Related Projects

- [FractalDataWorks Enhanced Enums](https://github.com/FractalDataWorks/enhanced-enums) - Type-safe enum pattern implementation with source generation
- [FractalDataWorks Developer Kit](https://github.com/FractalDataWorks/developer-kit) - Development tools and utilities
- [FractalDataWorks Code Builder](https://github.com/FractalDataWorks/code-builder) - Fluent API for programmatic code generation

## Requirements

- .NET Standard 2.0 or higher
- Compatible with:
  - .NET Framework 4.6.1+
  - .NET Core 2.0+
  - .NET 5.0+

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## Support

For issues, questions, or suggestions, please file an issue on the [GitHub repository](https://github.com/FractalDataWorks/fractaldataworks-net).