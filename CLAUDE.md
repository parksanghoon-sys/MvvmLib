# CLAUDE.md - Instructions for Claude AI Assistant

## Project Overview
This is MvvmLib, a C#/.NET MVVM library with WPF example applications. The project includes:
- Core MVVM library components
- WPF code checking/comparison tools
- Project change tracking functionality
- Example applications demonstrating MVVM patterns

## Development Environment Setup
- Platform: Windows (win32)
- Primary Language: C#/.NET
- UI Framework: WPF
- Version Control: Git (main branch: master)

## Build & Test Commands
```bash
# Build the solution
dotnet build

# Run tests (if available)
dotnet test

# Clean build artifacts
dotnet clean
```

## Code Style & Conventions
- Follow standard C# naming conventions (PascalCase for public members, camelCase for private)
- Use proper MVVM patterns with ViewModels, Commands, and data binding
- Maintain separation of concerns between UI, business logic, and data layers
- Use dependency injection where appropriate
- Follow existing namespace structure and organization

## Key Directories
- `/example/` - Example WPF applications demonstrating library usage
- `/example/wpfCodeCheck/` - Code comparison and checking tools
- Core library files are in the root directory structure

## Git Workflow
- Main branch: `master`
- Current working branch: `bug/fix-1`
- Always check git status before making changes
- Create meaningful commit messages
- Run builds and tests before committing

## Common Tasks
### File Analysis
- Use Glob/Grep tools to find specific files or code patterns
- Read files completely to understand context before making changes
- Check for dependencies and imports when modifying code

### Code Editing
- Always read the file first to understand existing patterns
- Follow existing code style and conventions
- Preserve existing using statements and namespace structure
- Test changes when possible

### Git Operations
- Check git status frequently
- Use meaningful commit messages
- Only commit when explicitly requested
- Push changes only when requested

## Troubleshooting
- Check for compilation errors after code changes
- Verify WPF binding and MVVM patterns are maintained
- Ensure proper exception handling in file operations
- Test UI responsiveness for long-running operations

## Notes for Claude
- This is a mature codebase with established patterns - follow existing conventions
- WPF applications require careful attention to UI thread operations
- File comparison and tracking features are core functionality - handle with care
- Always verify changes don't break existing MVVM bindings or command patterns