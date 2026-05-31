import { useState } from 'react';
import './CollectionModal.css';
import {getHttpErrorMessage, getServerErrorMessage} from "../../api/errorHandler.js";
import i18n from "i18next";

export default function CollectionModal({
                                                  isOpen,
                                                  onClose,
                                                  onSubmit,
                                                  t,
                                                  initialLabel = '',
                                                  isEditing = false
                                              }) {
    const [label, setLabel] = useState(initialLabel);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!label.trim()) {
            setError(t('CollectionModal.LabelRequired'));
            return;
        }

        setLoading(true);
        setError(null);

        try {
            await onSubmit(label);
            setLabel('');
            onClose();
        } catch (err) {
            if (err.error) {
                setError(getServerErrorMessage(err.error, t, i18n, 'CollectionModal'));
                return;
            }
            setError(getHttpErrorMessage(err.status, t));
        } finally {
            setLoading(false);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content glass" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <h2>{isEditing ? t('CollectionModal.EditTitle') : t('CollectionModal.Title')}</h2>
                    <button
                        className="modal-close"
                        onClick={onClose}
                        aria-label="Fermer"
                    >
                        <i className="ti ti-x"></i>
                    </button>
                </div>

                <form onSubmit={handleSubmit}>
                    <div className="input-wrap">
                        <i className="ti ti-tag" aria-hidden="true"></i>
                        <input
                            type="text"
                            name="collection-label"
                            id="collection-label"
                            value={label}
                            onChange={(e) => setLabel(e.target.value)}
                            placeholder={t('CollectionModal.Placeholder')}
                            disabled={loading}
                        />
                    </div>

                    {error && <div className="error-message">{error}</div>}

                    <div className="modal-footer">
                        <button
                            type="button"
                            onClick={onClose}
                            disabled={loading}
                            className="btn-cancel"
                        >
                            {t('CollectionModal.Cancel')}
                        </button>
                        <button
                            type="submit"
                            disabled={loading}
                            className="btn-primary"
                        >
                            {loading ? (t('CollectionModal.Saving')) : (isEditing ? t('CollectionModal.Save') : t('CollectionModal.Create'))}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}