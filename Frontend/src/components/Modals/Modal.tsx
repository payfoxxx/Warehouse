import React, { useEffect } from "react";
import '../../css/modal.css';

interface ModalProps {
    isOpen: boolean;
    onClose: ()=> void;
    title: string | React.ReactNode;
    children: React.ReactNode;
    actions?: React.ReactNode;
    className?: string;
}

const Modal: React.FC<ModalProps> = ({
    isOpen,
    onClose,
    title,
    children,
    actions,
    className =''
}) => {
    useEffect(() => {
        if (isOpen) {
            document.body.style.overflow = 'hidden';
        } else {
            document.body.style.overflow = 'auto';
        }

        return () => {
            document.body.style.overflow = 'auto';
        }
    }, [isOpen])

    if (!isOpen) return null;

    return (
        <div className={`modal-overlay ${className !== '' ? className + '-overlay' : ''} ${isOpen ? 'active' : ''}`}>
            <div className={`modal ${className}`}>
                <button className="modal-close" onClick={onClose}>&times;</button>
                <div className="modal-content">
                    <div className="modal-header">
                        <h2>{title}</h2>
                    </div>
                    <div className="modal-body">
                        {children}
                    </div>
                    {actions && (
                        <div className="modal-actions">
                            {actions}
                        </div>
                    )}
                </div>
            </div>
        </div>
    )
}

export default Modal;