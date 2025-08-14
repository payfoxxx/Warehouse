import React, { useState, useEffect } from "react";
import { clientService } from "../services/clientService";
import type { ClientCreateDto, ClientReadDto, ClientUpdateDto } from "../types/client";
import '../css/modal.css';
import { handleApiError } from "../utils/errorHandler";
import ErrorModal from "../components/Modals/ErrorModal";
import Modal from "../components/Modals/Modal";
import '../css/directory.css'

const ClientsPage: React.FC = () => {
    const [clients, setClients] = useState<ClientReadDto[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [showByState, setShowByState] = useState<number>(1);
    const [modalOpen, setModalOpen] = useState<"create" | "edit" | null>(null);
    const [currentClient, setCurrentClient] = useState<ClientUpdateDto | null>(null);
    const [newClient, setNewClient] = useState<ClientCreateDto>({ name: "", address: "" });
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        fetchClients();
    }, [showByState]);

    const fetchClients = async () => {
        setLoading(true);
        try {
            const data = await clientService.fetchAll(showByState);
            setClients(data.data);
        } catch (error) {
            console.error("Ошибка при получении клиентов:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        } finally {
            setLoading(false);
        }
    };

    const handleCreate = async () => {
        if (!newClient.name.trim()) {
            setError("Имя клиента обязательно");
            return;
        }
        if (!newClient.address.trim()) {
            setError("Адрес клиента обязательно");
            return;
        }
        try {
            await clientService.create(newClient);
            setModalOpen(null);
            setNewClient({ name: "", address: "" });
            fetchClients();
        } catch (error) {
            console.error("Ошибка при создании клиента:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    };

    const handleUpdate = async () => {
        if (!currentClient) return;
        if (!currentClient.name.trim()) {
            setError("Имя клиента обязательно");
            return;
        }
        if (!currentClient.address.trim()) {
            setError("Адрес клиента обязательно");
            return;
        }
        try {
            console.log(currentClient.state);
            await clientService.update(currentClient.id, currentClient);
            setModalOpen(null);
            fetchClients();
        } catch (error) {
            console.error("Ошибка при обновлении клиента:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    };

    const handleDelete = async () => {
        try {
            if (currentClient == null) return;
            await clientService.delete(currentClient.id);
            setModalOpen(null);
            fetchClients();
        } catch (error) {
            console.error("Ошибка при удалении клиента:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    }

    const handleRowClick = async (id: string) => {
        try {
            const client = await clientService.getById(id);
            setCurrentClient(client.data);
            setModalOpen("edit");
        } catch (error) {
            console.error("Ошибка при получении клиента:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    };

    return (
        <div className="directory-container">
            <div className="directory-header">
                <h1>Клиенты {showByState == 2 && '(Архив)'}</h1>

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
            ) : clients.length === 0 ? (
                <div className="no-data">
                    {showByState == 2 ? 'Архив пуст' : 'Нет данных'}
                </div>
            ) : (
                <table className="directory-table">
                    <thead>
                        <tr>
                            <th>Название</th>
                            <th>Адрес</th>
                        </tr>
                    </thead>
                    <tbody>
                        {clients.map((client) => (
                            <tr key={client.id} onClick={() => handleRowClick(client.id)}>
                                <td>{client.name}</td>
                                <td>{client.address}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}

            <Modal
                isOpen={modalOpen === 'create'}
                onClose={() => setModalOpen(null)}
                title="Создать клиента"
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
                        value={newClient.name}
                        onChange={(e) => setNewClient({ ...newClient, name: e.target.value })}
                        placeholder="Имя"
                    />
                </div>
                <div>
                    <input 
                        type="text"
                        value={newClient.address}
                        onChange={(e) => setNewClient({ ...newClient, address: e.target.value })}
                        placeholder="Адрес"
                    />
                </div>
            </Modal>

            {currentClient && (
                <Modal
                    isOpen={modalOpen === 'edit'}
                    onClose={() => setModalOpen(null)}
                    title="Редактировать клиента"
                    actions={
                        <>
                            <button onClick={handleUpdate}>Сохранить</button>
                            <button onClick={handleDelete}>Удалить</button>
                            <button onClick={() => setModalOpen(null)}>Отмена</button>
                        </>
                    }
                >
                    <div>
                        <div>
                            <input
                                type="text"
                                value={currentClient.name}
                                onChange={(e) => setCurrentClient({ ...currentClient, name: e.target.value })}
                                placeholder="Имя"
                            />
                        </div>
                        <div>
                            <input 
                                type="text"
                                value={currentClient.address}
                                onChange={(e) => setCurrentClient({ ...currentClient, address: e.target.value })}
                                placeholder="Адрес"
                            />
                        </div>
                        
                        <div className="state-selection">
                            <label>Состояние:</label>
                            <select
                                value={currentClient.state}
                                onChange={(e) => setCurrentClient({ ...currentClient, state: Number(e.target.value) })}
                            >
                                <option value={Number(1)}>В работе</option>
                                <option value={Number(2)}>Архив</option>
                            </select>
                        </div>
                    </div>
                </Modal>
            )}    

            {/* {modalOpen === 'create' && (
                <div className="modal">
                    <div className="modal-content">
                        <h2>Создать клиента</h2>
                        <input
                            type="text"
                            value={newClient.name}
                            onChange={(e) => setNewClient({ ...newClient, name: e.target.value })}
                            placeholder="Название"
                        />
                        <input 
                            type="text"
                            value={newClient.address}
                            onChange={(e) => setNewClient({ ...newClient, address: e.target.value })}
                            placeholder="Адрес"
                        />

                        <div className="modal-actions">
                            <button onClick={() => setModalOpen(null)}>Отмена</button>
                            <button onClick={handleCreate}>Создать</button>
                        </div>
                    </div>
                </div>
            )}

            {modalOpen === 'edit' && currentClient && (
                <div className="modal">
                    <div className="modal-content">
                        <h2>Редактировать клиента</h2>
                        <input
                            type="text"
                            value={currentClient.name}
                            onChange={(e) => setCurrentClient({ ...currentClient, name: e.target.value })}
                            placeholder="Название"
                        />
                        <input
                            type="text"
                            value={currentClient.address}
                            onChange={(e) => setCurrentClient({ ...currentClient, address: e.target.value })}
                            placeholder="Адрес"
                        />
                        <div className="state-selection">
                            <label>Состояние:</label>
                            <select
                                value={1}
                                onChange={(e) => setCurrentClient({ ...currentClient, state: Number(e.target.value) })}
                            >
                                <option value={Number(1)}>В работе</option>
                                <option value={Number(2)}>Архив</option>
                            </select>
                        </div>
                        <div className="modal-actions">
                            <button onClick={handleUpdate}>Сохранить</button>
                            <button onClick={handleDelete}>Удалить</button>
                            <button onClick={() => setModalOpen(null)}>Отмена</button>
                        </div>
                    </div>
                </div>
            )} */}
        </div>
    ) 
}

export default ClientsPage;