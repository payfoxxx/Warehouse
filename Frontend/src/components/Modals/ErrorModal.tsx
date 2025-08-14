import React from "react";
import Modal from "./Modal";

interface ErrorModalProps {
    isOpen: boolean;
    onClose: () => void;
    message: string;
}

const ErrorModal: React.FC<ErrorModalProps> = ({ isOpen, onClose, message }) => {
    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title={
                <>
                    <span className="error-icon">!</span>
                    Ошибка
                </>
            }
            className="error-modal"
        >
            <p>{message}</p>
            <div className="modal-actions">
                <button className="confirm-btn" onClick={onClose}>Ok</button>
            </div>
        </Modal>
    )
};

export default ErrorModal;