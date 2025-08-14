import React from 'react'
import './App.css'
import { BrowserRouter as Router, Routes, Route } from 'react-router';
import Sidebar from './components/Sidebar/sidebar';
import HomePage from './pages/HomePage';
import ClientsPage from './pages/ClientsPage';
import ResourcesPage from './pages/ResourcesPage';
import MeasureUnitsPage from './pages/MeasureUnitPage';
import ReceiptPage from './pages/ReceiptPage';
import BalancePage from './pages/BalancePage';
import ShipmentPage from './pages/ShipmentPage';

const App: React.FC = () => {
  return (
    <Router>
      <div className="app-container">
        <Sidebar />
        <main className="main-content">
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path='/clients' element={<ClientsPage />} />
            <Route path='/resources' element={<ResourcesPage />} />
            <Route path='/measureunits' element={<MeasureUnitsPage />} />
            <Route path='/receipts' element={<ReceiptPage />} />
            <Route path='/balances' element={<BalancePage />} />
            <Route path='/shipments' element={<ShipmentPage />} />
            {/* Добавьте здесь маршруты для других страниц */}
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App
