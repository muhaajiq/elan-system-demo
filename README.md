# ELAN – Employee Onboarding System (Sanitised Demo)
# Important Notice
This repository contains a sanitised version of a project I worked on during my previous employment.

Due to confidentiality and intellectual property policies:
- Proprietary libraries (DLLs) and internal dependencies have been removed
- Company-specific names and identifiers have been replaced
- Some configurations and integrations are intentionally incomplete

As a result, this solution is not expected to compile or run fully.

This repository is shared strictly for:
- Demonstrating code structure
- Showcasing architecture and design approach
- Reviewing coding standards and patterns

# Project Overview
ELAN (Employee Lifecycle & Onboarding System) is an internal enterprise web application designed to manage employee onboarding workflows.
The system supports:
- Submission and tracking of onboarding requests
- Multi-level approval workflows
- Task management for different roles (reviewers, approvers)
- Administrative configuration and data management

# Architecture Overview
The solution follows a layered architecture using .NET and Blazor.
1. Business Layer (Business Project)
- Contains all core business logic
- Handles workflow rules, validations, and process orchestration
- Acts as the main bridge between UI and data access

2. Data Layer (Data Project)
- Responsible for data access and persistence
- Communicates with the database
- Encapsulates queries and repository logic

3. Presentation Layer (Blazor Project)
- Built using Blazor (.razor components)
- Handles UI rendering and user interaction
- Includes:
	- Pages
	- Reusable components
	- Partial components

4. Shared Components
- Entities Project
- Contains:
	- View Models
	- Data Transfer Objects (DTOs)
	- Entity definitions used across layers
- Framework Project
- Contains:
	- Helper utilities
	- Common reusable functions
	- Shared abstractions

# Notes for Reviewers
Some dependencies and integrations are intentionally missing
Certain functionalities may appear incomplete due to removed references
Focus areas for review:
- Code quality
- Structure
- Design patterns
- Problem-solving approach
