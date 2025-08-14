import React from "react";
import { Link } from "react-router";
import './sidebar.css';

const Sidebar: React.FC = () => {
    return (
        <nav className="sidebar">
            <div className="sidebar-header">
                <Link to="/" className="sidebar-title">
                    Управление складом
                </Link>
            </div>

            <div className="sidebar-section">
                <h3 className="sidebar-subtitle">
                    Склад
                </h3>
                <ul className="sidebar-menu">
                    <li><Link to="/balances">Баланс</Link></li>
                    <li><Link to="/receipts">Поступления</Link></li>
                    <li><Link to="/shipments">Отгрузки</Link></li>
                </ul>
            </div>

            <div className="sidebar-section">
                <h3 className="sidebar-subtitle">
                    Справочники
                </h3>
                <ul className="sidebar-menu">
                    <li><Link to="/clients">Клиенты</Link></li>
                    <li><Link to="/measureunits">Единицы измерения</Link></li>
                    <li><Link to="/resources">Ресурсы</Link></li>
                </ul>
            </div>
        </nav>
    )
}
export default Sidebar;