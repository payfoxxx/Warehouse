# 🏭 Система управления складом
Тестовый проект на React TS + .NET 8 WebAPI.

## 🛠 Технологический стек
### Бэкенд
- **.NET 8 WebAPI** - высокопроизводительная серверная платформа
- **Entity Framework Core** - ORM для работы с базой данных
- **Npgsql** - драйвер PostgreSQL для .NET

### Фронтенд
- **Vite** - инструмент сборки
- **React** - библиотека для построения UI
- **TypeScript** - статическая типизация
- **Axios** - работа с API

### База данных
- **PostgreSQL 16** - реляционная СУБД
  - Таблицы: `Balances`, `Clients`, `MeasureUnits`, `ReceiptDocuments`, `ReceiptResources`, `Resources`, `ShipmentDocuments`, `ShipmentResources`

## ⚙️ Функциональные возможности
- CRUD операции
- Контроль баланса
- Фильтрация и сортировка данных

## 🚀 Запуск проекта
### Предварительные требования
- Node.js v18+
- .NET 8 SDK
- PostgreSQL 15+

### Установка
1. **Клонировать репозиторий:**
   git clone https://github.com/payfoxxx/Warehouse.git
2. **Настройка базы данных:**	
	Обновить строку подключения в appsettings.json на свои данные
3. **Запуск Backend**
Осуществляется с помощью команды `dotnet run` в директории `Backend/Backend`
4. **Запуск Frontend**
Осуществляется с помощью команда `npm run dev` в директории `Frontend`