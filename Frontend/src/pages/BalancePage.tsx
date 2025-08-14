import React, { useState, useEffect } from "react";
import { balanceService } from "../services/balanceService";
import { measureUnitService } from "../services/measureUnitService";
import { resourceService } from "../services/resourceService";
import type { BalanceReadDto, BalanceFilterDto } from "../types/balance";
import { handleApiError } from "../utils/errorHandler";
import MultiSelect from "../components/MultiSelect";
import '../css/directory.css'

const BalancePage: React.FC = () => {
    const [balanceItems, setBalanceItems] = useState<BalanceReadDto[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [filters, setFilters] = useState<BalanceFilterDto>({});
    const [resources, setResources] = useState<{ id: string; name: string }[]>([]);
    const [measureUnits, setMeasureUnits] = useState<{ id: string; name: string}[]>([]);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                const [balance, res, units] = await Promise.all([
                    balanceService.fetchAll(filters),
                    resourceService.fetchAll(1),
                    measureUnitService.fetchAll(1)
                ]);
                setBalanceItems(balance.data);
                setResources(res.data);
                setMeasureUnits(units.data);
            } catch (error) {
                console.error("Ошибка загрузки данных");
                const errorMessage = handleApiError(error);
                setError(`${errorMessage}.`)
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [filters]);

    const handleMultiSelectChange = (name: keyof BalanceFilterDto, values: string[]) => {
        setFilters(prev => ({ ...prev, [name]: values }));
    };

    const handleFilterChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFilters(prev => ({ ...prev, [name]: value }));
    }

    return (
        <div className="directory-container">
            <div className="directory-header">
                <h1>Баланс</h1>

                <div className="filters">
                    <MultiSelect
                        options={resources.map(r => ({ value: r.id, label: r.name }))}
                        value={filters.resourceId || []}
                        onChange={(values) => handleMultiSelectChange('resourceId', values)}
                        placeholder="Все ресурсы"
                        closeMenuOnSelect={false}
                    />
                    <MultiSelect
                        options={measureUnits.map(mu => ({ value: mu.id, label: mu.name }))}
                        value={filters.measureUnitId || []}
                        onChange={(values) => handleMultiSelectChange('measureUnitId', values)}
                        placeholder="Все единицы"
                        closeMenuOnSelect={false}
                    />
                </div>
            </div>

            {loading ? (
                <div className="loading-indicator">Загрузка</div>
            // ) : error ? (
                // <div className="error-message">{error}</div>
            ) : balanceItems.length === 0 ? (
                <div className="no-data">
                    'Нет данных'
                </div>
            ) : (
                <table className="directory-table">
                    <thead>
                        <tr>
                            <th>Ресурс</th>
                            <th>Единица измерения</th>
                            <th>Количество</th>
                        </tr>
                    </thead>
                    <tbody>
                        {balanceItems.map(item => (
                            <tr key={`${item.resourceId}-${item.measureUnitId}`}>
                                <td>{item.resourceName}</td>
                                <td>{item.measureUnitName}</td>
                                <td>{item.count}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    )
}

export default BalancePage;