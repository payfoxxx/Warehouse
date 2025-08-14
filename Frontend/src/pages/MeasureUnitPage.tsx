import React, { useState, useEffect } from "react";
import { measureUnitService } from "../services/measureUnitService";
import { handleApiError } from "../utils/errorHandler";
import type { MeasureUnitCreateDto, MeasureUnitReadDto, MeasureUnitUpdateDto } from "../types/measureUnit";
import '../css/modal.css';
import Modal from "../components/Modals/Modal";
import ErrorModal from "../components/Modals/ErrorModal";
import '../css/directory.css'

const MeasureUnitsPage: React.FC = () => {
    const [measureUnits, setMeasureUnits] = useState<MeasureUnitReadDto[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [showByState, setShowByState] = useState<number>(1);
    const [modalOpen, setModalOpen] = useState<"create" | "edit" | null>(null);
    const [currentMeasureUnit, setCurrentMeasureUnit] = useState<MeasureUnitUpdateDto | null>(null);
    const [newMeasureUnit, setNewMeasureUnit] = useState<MeasureUnitCreateDto>({ name: "" });
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        fetchMeasureUnits();
    }, [showByState]);

    const fetchMeasureUnits = async () => {
        setLoading(true);
        try {
            const data = await measureUnitService.fetchAll(showByState);
            setMeasureUnits(data.data);
        } catch (error) {
            console.error("Ошибка при получении клиентов:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        } finally {
            setLoading(false);
        }
    };

    const handleCreate = async () => {
        if (!newMeasureUnit.name.trim()) {
            setError("Название единицы измерения обязательно");
            return;
        }
        
        try {
            await measureUnitService.create(newMeasureUnit);
            setModalOpen(null);
            setNewMeasureUnit({ name: "" });
            fetchMeasureUnits();
        } catch (error) {
            console.error("Ошибка при создании единицы измерения:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    };

    const handleUpdate = async () => {
        if (!currentMeasureUnit) return;
        if (!currentMeasureUnit.name.trim()) {
            setError("Название единицы измерения обязательно");
            return;
        }

        try {
            console.log(currentMeasureUnit.state);
            await measureUnitService.update(currentMeasureUnit.id, currentMeasureUnit);
            setModalOpen(null);
            fetchMeasureUnits();
        } catch (error) {
            console.error("Ошибка при редактировании единицы измерения:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    };

    const handleDelete = async () => {
        try {
            if (currentMeasureUnit == null) return;
            await measureUnitService.delete(currentMeasureUnit.id);
            setModalOpen(null);
            fetchMeasureUnits();
        } catch (error) {
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    }

    const handleRowClick = async (id: string) => {
        try {
            const measureUnit = await measureUnitService.getById(id);
            setCurrentMeasureUnit(measureUnit.data);
            setModalOpen("edit");
        } catch (error) {
            console.error("Ошибка при получении единицы измерения:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    };

    return (
        <div className="directory-container">
            <div className="directory-header">
                <h1>Единицы измерения {showByState == 2 && '(Архив)'}</h1>

                <ErrorModal
                    isOpen={error !== null}    
                    onClose={() => setError(null)}
                    message={error || ''}
                />

                <div className="directory-actions">
                    {showByState == 1 && (
                        <button
                            className="add-button"
                            onClick={() => setModalOpen("create")}
                        >
                            Добавить
                        </button>
                    )}

                    <button
                        className="archive-toggle-button"
                        onClick={() => { 
                            if (showByState == 1) setShowByState(2) 
                            else setShowByState(1)
                            }}
                    >
                        {showByState == 1 ? 'К архиву' : 'К рабочим'}
                    </button>
                </div>
            </div>

            {loading ? (
                <div className="loading-indicator">Загрузка</div>
            // ) : error ? (
                // <div className="error-message">{error}</div>
            ) : measureUnits.length === 0 ? (
                <div className="no-data">
                    {showByState == 2 ? 'Архив пуст' : 'Нет данных'}
                </div>
            ) : (
                <table className="directory-table">
                    <thead>
                        <tr>
                            <th>Название</th>
                        </tr>
                    </thead>
                    <tbody>
                        {measureUnits.map((measureUnit) => (
                            <tr key={measureUnit.id} onClick={() => handleRowClick(measureUnit.id)}>
                                <td>{measureUnit.name}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}

            <Modal
                isOpen={modalOpen === 'create'}
                onClose={() => setModalOpen(null)}
                title="Создать единицу измерения"
                actions={
                    <>
                        <button className="confirm-btn" onClick={handleCreate}>Создать</button>
                        <button className="cancel-btn" onClick={() => setModalOpen(null)}>Отмена</button>
                    </>
                }
            >
                <div>
                    <input
                        type="text"
                        value={newMeasureUnit.name}
                        onChange={(e) => setNewMeasureUnit({ ...newMeasureUnit, name: e.target.value })}
                        placeholder="Название"
                    />
                </div>
            </Modal>

            {currentMeasureUnit && (
                <Modal
                    isOpen={modalOpen === 'edit'}
                    onClose={() => setModalOpen(null)}
                    title="Редактировать единицу измерения"
                    actions={
                        <>
                            <button onClick={handleUpdate}>Сохранить</button>
                            <button onClick={handleDelete}>Удалить</button>
                            <button onClick={() => setModalOpen(null)}>Отмена</button>
                        </>
                    }
                >
                    <div>
                        <input
                            type="text"
                            value={currentMeasureUnit.name}
                            onChange={(e) => setCurrentMeasureUnit({ ...currentMeasureUnit, name: e.target.value })}
                            placeholder="Название"
                        />
                        <div className="state-selection">
                            <label>Состояние:</label>
                            <select
                                value={currentMeasureUnit.state}
                                onChange={(e) => setCurrentMeasureUnit({ ...currentMeasureUnit, state: Number(e.target.value) })}
                            >
                                <option value={Number(1)}>В работе</option>
                                <option value={Number(2)}>Архив</option>
                            </select>
                        </div>
                    </div>
                </Modal>
            )}    
        </div>
    ) 
}

export default MeasureUnitsPage;