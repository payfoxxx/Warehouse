import React, { useState, useEffect } from "react";
import { receiptService } from "../services/receiptService";
import { resourceService } from "../services/resourceService";
import { measureUnitService } from "../services/measureUnitService";
import type { ReceiptDocumentCreateDto, ReceiptDocumentReadDto, ReceiptDocumentUpdateDto, ReceiptDocumentFilter, ReceiptResourceUpdateDto } from "../types/receipt";
import '../css/modal.css';
import { handleApiError } from "../utils/errorHandler";
import ErrorModal from "../components/Modals/ErrorModal";
import Modal from "../components/Modals/Modal";
import MultiSelect from '../components/MultiSelect';
import '../css/directory.css'

const ReceiptPage: React.FC = () => {
    const [documents, setDocuments] = useState<ReceiptDocumentReadDto[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [modalOpen, setModalOpen] = useState<"create" | "edit" | null>(null);
    const [currentDocument, setCurrentDocument] = useState<ReceiptDocumentUpdateDto | null>(null);
    const [newDocument, setNewDocument] = useState<ReceiptDocumentCreateDto>({ 
        num: "", 
        date: new Date().toISOString(), 
        receiptResources: [] as { resourceId: string, measureUnitId: string, count: number}[] });
    const [resources, setResources] = useState<{ id: string; name: string }[]>([]);
    const [measureUnits, setMeasureUnits] = useState<{ id: string; name: string}[]>([]);
    const [filters, setFilters] = useState<ReceiptDocumentFilter>({});
    const [allDocuments, setAllDocuments] = useState<ReceiptDocumentUpdateDto[]>([]);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                const [docs, res, units, allDocs] = await Promise.all([
                    receiptService.fetchAll(filters),
                    resourceService.fetchAll(1),
                    measureUnitService.fetchAll(1),
                    receiptService.fetchAll({})
                ]);
                setDocuments(docs.data);
                setResources(res.data);
                setMeasureUnits(units.data);
                setAllDocuments(allDocs.data);
            } catch (error) {
                console.error("Ошибка загрузки данных:", error);
                const errorMessage = handleApiError(error);
                setError(`${errorMessage}.`)
            } finally {
                setLoading(false);
            }
        }

        fetchData();
    }, [filters]);

    const handleCreate = async () => {
        if (!newDocument.num.trim()) {
            setError("Номер документа обязателен");
            return;
        }

        if (!newDocument.date.trim()) {
            setError("Дата документа обязательна");
            return;
        }

        if (newDocument.receiptResources.length !== 0) {
            for (const [index, resource] of newDocument.receiptResources.entries()) {
                if (!resource.resourceId) {
                    setError(`Выберите ресурс в строке ${index + 1}`);
                    return;
                }

                if (!resource.measureUnitId) {
                    setError(`Выберите единицу измерения в строке ${index + 1}`);
                    return;
                }

                if (resource.count <= 0) {
                    setError(`Количество должно быть больше 0 в строке ${index + 1}`);
                    return;
                }
            }
        }
        
        try {
            await receiptService.create(newDocument);
            setModalOpen(null);
            setNewDocument({ 
                num: "", 
                date: new Date().toISOString(), 
                receiptResources: [] as { resourceId: string, measureUnitId: string, count: number}[] 
            });
            const docs = await receiptService.fetchAll(filters);
            setDocuments(docs.data);
        } catch (error) {
            console.error("Ошибка при создании документа:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    };

    const handleUpdate = async () => {
        if (!currentDocument) return;
        
        if (!currentDocument.num.trim()) {
            setError("Номер документа обязателен");
            return;
        }

        if (!currentDocument.date.trim()) {
            setError("Дата документа обязательна");
            return;
        }

        if (currentDocument.receiptResources.length !== 0) {
            for (const [index, resource] of currentDocument.receiptResources.entries()) {
                if (!resource.resourceId) {
                    setError(`Выберите ресурс в строке ${index + 1}`);
                    return;
                }

                if (!resource.measureUnitId) {
                    setError(`Выберите единицу измерения в строке ${index + 1}`);
                    return;
                }

                if (resource.count <= 0) {
                    setError(`Количество должно быть больше 0 в строке ${index + 1}`);
                    return;
                }
            }
        }

        try {
            await receiptService.update(currentDocument.id, currentDocument);
            setModalOpen(null);
            const docs = await receiptService.fetchAll(filters);
            setDocuments(docs.data);
        } catch (error) {
            console.error("Ошибка при обновлении документа:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    };

    const handleDelete = async () => {
        try {
            if (currentDocument == null) return;
            await receiptService.delete(currentDocument.id);
            setModalOpen(null);
            const docs = await receiptService.fetchAll(filters);
            setDocuments(docs.data);
        } catch (error) {
            console.error("Ошибка при удалении документа:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    }

    const handleRowClick = async (id: string) => {
        try {
            const document = await receiptService.getById(id);
            setCurrentDocument(document.data);
            console.log(currentDocument);
            setModalOpen("edit");
        } catch (error) {
            console.error("Ошибка при получении документа:", error);
            const errorMessage = handleApiError(error);
            setError(`${errorMessage}.`)
        }
    };

    const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        if (name === "dateFrom" || name === "dateTo") {
            if (value) {
                const date = new Date(value);

                const utcDate = new Date(Date.UTC(
                    date.getFullYear(),
                    date.getMonth(),
                    date.getDate(),
                    name === "dateTo" ? 23 : 0,
                    name === "dateTo" ? 59 : 0,
                    name === "dateTo" ? 59 : 0,
                    name === "dateTo" ? 999 : 0
                ));

                setFilters(prev => ({
                    ...prev,
                    [name]: utcDate.toISOString()
                }))
            } else {
                setFilters(prev => ({
                    ...prev,
                    [name]: undefined
                }));
            }
        } else
            setFilters(prev => ({ ...prev, [name]: value }));
    }

    const handleMultiSelectChange = (name: keyof ReceiptDocumentFilter, values: string[]) => {
        setFilters(prev => ({ ...prev, [name]: values }));
    };

    const addResource = (isNew = true) => {
        if (isNew) {
            setNewDocument(prev => ({
                ...prev,
                receiptResources: [...prev.receiptResources, { resourceId: "", measureUnitId: "", count: 0}]
            }))
        } else if (currentDocument) {
            setCurrentDocument(prev => prev? {
                ...prev,
                receiptResources: [...prev.receiptResources, {
                    id: "",
                    resourceId: "",
                    measureUnitId: "",
                    count: 0,
                }]
            } : null)
        }
    };

    const removeResource = (index: number, isNew = true) => {
        if (isNew) {
            setNewDocument(prev => ({
                ...prev,
                receiptResources: prev.receiptResources.filter((_, i) => i !== index)
            })) 
        } else if (currentDocument) {
            setCurrentDocument(prev => prev ? {
                ...prev,
                receiptResources: prev.receiptResources.filter((_, i) => i !== index)
            } : null)
        }
    }

    const handleResourceChange = (
        index: number,
        field: keyof ReceiptResourceUpdateDto,
        value: string | number,
        isNew = true
    ) => {
        if (isNew) {
            setNewDocument(prev => {
                const newResources = [...prev.receiptResources];
                newResources[index] = { ...newResources[index], [field]:value };
                return { ...prev, receiptResources: newResources };
            })
        } else if (currentDocument) {
            setCurrentDocument(prev => {
                if (!prev) return null;
                const newResources = [...prev.receiptResources];
                newResources[index] = { ...newResources[index], [field]:value };
                return { ...prev, receiptResources: newResources };
            })
        }
    }

    return (
        <div className="directory-container">
            <div className="directory-header">
                <h1>Поступления</h1>

                <ErrorModal
                    isOpen={error !== null}    
                    onClose={() => setError(null)}
                    message={error || ''}
                />

                <div className="filters">
                    <input 
                        type="date"
                        name="dateFrom"
                        onChange={handleFilterChange}
                        placeholder="Дата от"
                    />
                    <input 
                        type="date"
                        name="dateTo"
                        onChange={handleFilterChange}
                        placeholder="Дата до"
                    />
                    <MultiSelect
                        options={allDocuments.map(d => ({ value: d.id, label: d.num }))}
                        value={filters.numId || []}
                        onChange={(values) => handleMultiSelectChange('numId', values)}
                        placeholder="Выберите документы"
                        closeMenuOnSelect={false}
                    />
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
                <div className="directory-actions">
                    <button
                        className="add-button"
                        onClick={() => setModalOpen("create")}
                    >
                        Добавить
                    </button>
                </div>
            </div>

            {loading ? (
                <div className="loading-indicator">Загрузка</div>
            // ) : error ? (
                // <div className="error-message">{error}</div>
            ) : documents.length === 0 ? (
                <div className="no-data">
                    'Нет данных'
                </div>
            ) : (
                <table className="document-table">
                    <thead>
                        <tr>
                            <th>Номер</th>
                            <th>Дата</th>
                            <th>Ресурс</th>
                            <th>Единица измерения</th>
                            <th>Количество</th>
                        </tr>
                    </thead>
                    <tbody>
                        {documents.map(document => {
                            const hasResources = document.receiptResources.length > 0;
                            return (
                                <React.Fragment key={document.id}>
                                    {hasResources && document.receiptResources.map((resource, index) => (
                                        <tr key={`${document.id}-${index}`} onClick={() => handleRowClick(document.id)}>
                                            {index === 0 && (
                                                <>
                                                    <td rowSpan={document.receiptResources.length || 1}>
                                                        {document.num}
                                                    </td>
                                                    <td rowSpan={document.receiptResources.length || 1}>
                                                        {document.date.toString()}
                                                    </td>
                                                </>
                                            )}
                                            <td>{resource.resourceName}</td>
                                            <td>{resource.measureUnitName}</td>
                                            <td>{resource.count}</td>
                                        </tr>
                                    ))}

                                    {!hasResources && (
                                        <tr key={`${document.id}-empty`} onClick={() => handleRowClick(document.id)}>
                                            <td>{document.num}</td>
                                            <td>{document.date.toString()}</td>
                                            <td colSpan='3' style={{ textAlign: "center" }}>Нет ресурсов</td>
                                        </tr>
                                    )}
                                </React.Fragment>      
                            )
                        } 
                        )}
                    </tbody>
                </table>
            )}

            <Modal
                isOpen={modalOpen === 'create'}
                onClose={() => setModalOpen(null)}
                title="Создать документ поступления"
                actions={
                    <>
                        <button className="confirm-btn" onClick={handleCreate}>Создать</button>
                        <button className="cancel-btn" onClick={() => setModalOpen(null)}>Отмена</button>
                    </>
                }
            >
                <div>
                    <label>Номер документа:</label>
                    <input
                        type="text"
                        value={newDocument.num}
                        onChange={e => setNewDocument({ ...newDocument, num: e.target.value })}
                    />
                </div>
                <div>
                    <label>Дата:</label>
                    <input 
                        type="date"
                        value={new Date(newDocument.date).toISOString().split('T')[0]}
                        onChange={e => {
                            if (e.target.valueAsDate) {
                                const utcDate = new Date(Date.UTC(
                                    e.target.valueAsDate.getFullYear(),
                                    e.target.valueAsDate.getMonth(),
                                    e.target.valueAsDate.getDate()
                                )).toISOString();
                                setNewDocument({ ...newDocument, date: utcDate})
                            }
                        }}
                    />
                </div>

                <h3>Ресурсы:</h3>
                <button onClick={() => addResource(true)}>Добавить ресурс</button>
                <table>
                    <thead>
                        <tr>
                            <th>Ресурс</th>
                            <th>Единица измерения</th>
                            <th>Количество</th>
                        </tr>
                    </thead>
                    <tbody>
                        {newDocument.receiptResources.map((resource, index) => (
                            <tr key={index}>
                                <td>
                                    <select 
                                        value={resource.resourceId}
                                        onChange={e => handleResourceChange(index, "resourceId", e.target.value, true)}
                                    >
                                        <option value={""}>Выберите ресурс</option>
                                        {resources.map(r => (
                                            <option key={r.id} value={r.id}>{r.name}</option>
                                        ))}
                                    </select>
                                </td>
                                <td>
                                    <select 
                                        value={resource.measureUnitId}
                                        onChange={e => handleResourceChange(index, "measureUnitId", e.target.value, true)}
                                    >
                                        <option value={""}>Выберите единицу измерения</option>
                                        {measureUnits.map(mu => (
                                            <option key={mu.id} value={mu.id}>{mu.name}</option>
                                        ))}
                                    </select>
                                </td>
                                <td>
                                    <input
                                        type="number"
                                        value={resource.count}
                                        onChange={e => handleResourceChange(index, "count", Number(e.target.value), true)}
                                        min={0}
                                    />
                                </td>
                                <td>
                                    <button onClick={() => removeResource(index, true)}>Удалить ресурс</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </Modal>

            {currentDocument && (
                <Modal
                    isOpen={modalOpen === 'edit'}
                    onClose={() => setModalOpen(null)}
                    title="Редактировать документ"
                    actions={
                        <>
                            <button onClick={handleUpdate}>Сохранить</button>
                            <button onClick={handleDelete}>Удалить</button>
                            <button onClick={() => setModalOpen(null)}>Отмена</button>
                        </>
                    }
                >
                    <div>
                        <label>Номер документа:</label>
                        <input
                            type="text"
                            value={currentDocument.num}
                            onChange={e => setCurrentDocument({ ...currentDocument, num: e.target.value })}
                        />
                    </div>
                    <div>
                        <label>Дата:</label>
                        <input 
                            type="date"
                            value={new Date(currentDocument.date).toISOString().split('T')[0]}
                            onChange={e => {
                                if (e.target.valueAsDate) {
                                    const utcDate = new Date(Date.UTC(
                                        e.target.valueAsDate.getFullYear(),
                                        e.target.valueAsDate.getMonth(),
                                        e.target.valueAsDate.getDate()
                                    )).toISOString();
                                    setCurrentDocument({ ...currentDocument, date: utcDate})
                                }
                            }}
                        />
                    </div>

                    <h3>Ресурсы:</h3>
                    <button onClick={() => addResource(false)}>Добавить ресурс</button>
                    <table>
                        <thead>
                            <tr>
                                <th>Ресурс</th>
                                <th>Единица измерения</th>
                                <th>Количество</th>
                            </tr>
                        </thead>
                        <tbody>
                            {currentDocument.receiptResources.map((resource, index) => (
                                <tr key={index}>
                                    <td>
                                        <select 
                                            value={resource.resourceId}
                                            onChange={e => handleResourceChange(index, "resourceId", e.target.value, false)}
                                        >
                                            <option value={""}>Выберите ресурс</option>
                                            {resources.map(r => (
                                                <option key={r.id} value={r.id}>{r.name}</option>
                                            ))}
                                        </select>
                                    </td>
                                    <td>
                                        <select 
                                            value={resource.measureUnitId}
                                            onChange={e => handleResourceChange(index, "measureUnitId", e.target.value, false)}
                                        >
                                            <option value={""}>Выберите единицу измерения</option>
                                            {measureUnits.map(mu => (
                                                <option key={mu.id} value={mu.id}>{mu.name}</option>
                                            ))}
                                        </select>
                                    </td>
                                    <td>
                                        <input
                                            type="number"
                                            value={resource.count}
                                            onChange={e => handleResourceChange(index, "count", Number(e.target.value), false)}
                                            min={0}
                                        />
                                    </td>
                                    <td>
                                        <button onClick={() => removeResource(index, false)}>Удалить ресурс</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </Modal>
            )}
        </div>
    ) 
}

export default ReceiptPage;