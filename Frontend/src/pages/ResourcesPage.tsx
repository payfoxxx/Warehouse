import React, { useState, useEffect } from "react";
import { resourceService } from "../services/resourceService";
import { handleApiError } from "../utils/errorHandler";
import type { ResourceCreateDto, ResourceReadDto, ResourceUpdateDto } from "../types/resource";
import ErrorModal from "../components/Modals/ErrorModal";
import Modal from "../components/Modals/Modal";
import '../css/directory.css'

const ResourcesPage: React.FC = () => {
    const [resources, setResources] = useState<ResourceReadDto[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [showByState, setShowByState] = useState<number>(1);
    const [modalOpen, setModalOpen] = useState<"create" | "edit" | null>(null);
    const [currentResource, setCurrentResource] = useState<ResourceUpdateDto | null>(null);
    const [newResource, setNewResource] = useState<ResourceCreateDto>({ name: "" });
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        fetchResources();
    }, [showByState]);

    const fetchResources = async () => {
        setLoading(true);
        try {
            const data = await resourceService.fetchAll(showByState);
            setResources(data.data);
        } catch (error) {
            console.error("Ошибка при получении ресурсов:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        } finally {
            setLoading(false);
        }
    };

    const handleCreate = async () => {
        if (!newResource.name.trim()) {
            setError("Название ресурса обязательно");
            return;
        }

        try {
            await resourceService.create(newResource);
            setModalOpen(null);
            setNewResource({ name: "" });
            fetchResources();
        } catch (error) {
            console.error("Ошибка при создании ресурса:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    };

    const handleUpdate = async () => {
        if (!currentResource) return;
        if (!currentResource.name.trim()) {
            setError("Название ресурса обязательно");
            return;
        }
        try {
            console.log(currentResource.state);
            await resourceService.update(currentResource.id, currentResource);
            setModalOpen(null);
            fetchResources();
        } catch (error) {
            console.error("Ошибка при обновлении ресурса:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    };

    const handleDelete = async () => {
        try {
            if (currentResource == null) return;
            await resourceService.delete(currentResource.id);
            setModalOpen(null);
            fetchResources();
        } catch (error) {
            console.error("Ошибка при удалении ресурса:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    }

    const handleRowClick = async (id: string) => {
        try {
            const resource = await resourceService.getById(id);
            console.log(resource.data);
            setCurrentResource(resource.data);
            setModalOpen("edit");
        } catch (error) {
            console.error("Ошибка при получении ресурса:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    };

    return (
        <div className="directory-container">
            <div className="directory-header">
                <h1>Ресурсы {showByState == 2 && '(Архив)'}</h1>

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
            ) : resources.length === 0 ? (
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
                        {resources.map((resource) => (
                            <tr key={resource.id} onClick={() => handleRowClick(resource.id)}>
                                <td>{resource.name}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}

            <Modal
                isOpen={modalOpen === 'create'}
                onClose={() => setModalOpen(null)}
                title="Создать ресурс"
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
                        value={newResource.name}
                        onChange={(e) => setNewResource({ ...newResource, name: e.target.value })}
                        placeholder="Название"
                    />
                </div>
            </Modal>

            {currentResource && (
                <Modal
                    isOpen={modalOpen === 'edit'}
                    onClose={() => setModalOpen(null)}
                    title="Редактировать ресурс"
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
                            value={currentResource.name}
                            onChange={(e) => setCurrentResource({ ...currentResource, name: e.target.value })}
                            placeholder="Название"
                        />
                        <div className="state-selection">
                            <label>Состояние:</label>
                            <select
                                value={currentResource.state}
                                onChange={(e) => setCurrentResource({ ...currentResource, state: Number(e.target.value) })}
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

export default ResourcesPage;