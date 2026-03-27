# Changelog

---

## [0.1.0] - Initial Draft
### Added
- Single file with basic cart flow (still experimental)

---

## [0.2.0] - Class and Properties
### Added
- Item class (properties: name, price, stock)
- CartItem class (properties: product, quantity)
- ActionHistory class (properties: actionType, affectedItem, previousQuantity)
## [0.2.1] - Cart Operations and Console UI
### Added
- InventoryManager and CartManager classes
- Console colors for better user experience
### Changed
- Menu choices use `byte` instead of `int`
## [0.2.2] - Internal Class
### Changed
- Moved classes outside of internal class
## [0.2.3] - Cart Manager
### Fixed
- Completed CartManager with add, remove, update, undo/redo methods
## [0.2.4] - Console User Interface
### Added
- ConsoleUserInterface for menu display
## [0.2.5] - Comments and Cart Operations
### Added
- Comments in code
- Functions for cart operations (still in one file)
- Main method to run classes and functions
## [0.2.6] - Removed undo and redo
### Removed
- Undo and Redo (buggy)
## [0.2.7] - Removed other calling medthods from switch case
### Removed
- Cases (4–9) in switch (no functionality yet, caused confusion)
---

## [0.3.0] - Separation of Concerns
### Changed
- Split code into multiple files
- Added CartModel, CartItemModel, CartLogic, CartItemLogic, CartService, CartItemService, CartRules, CartItemRules, and ClassDiagram
- Clearer responsibilities

---

## [0.4.0] - Improve Model, Data Logic, and Business Logic
### Added
- ICartDataLogic interface
- InMemoryCartData storage
- CartJsonData integration for persistence
- CartManager for cart operations
- CartExceptions for error handling
### Changed
- CartDataLogic used in CartLogic and CartItemLogic
- CartItemModel merged into CartModel
- Removed ConsoleUI from Business Logic
- CartItem much focused on adding and removing
### Removed
- CartItemModel, CartLogic, CartItemLogic, CartItemService, CartItemRules

---

## [0.5.0] - Naming and Lambda Expressions
### Changed
- Improved naming, used PascalCase for classes and methods
- Used full-form lambda instead of shorthand (since it clearer at the time)

---

## [0.6.0] - Database Integration
### Added
- Database integration
- CartDBData class for database operations
- Data annotation for price in CartItemModel
### Changed
- Used lambda operators (`=>`) instead of full form (now understood)
- Used if-else instead of null conditional and coalescing operators
### Trying
- Encapsulation in Cart Model class