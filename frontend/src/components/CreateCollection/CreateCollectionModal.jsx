import { useState } from 'react';
import './CreateCollectionModal.css';

export default function CreateCollectionModal({ isOpen, onClose, onSubmit, t }) {
    const [label, setLabel] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!label.trim()) {
            setError(t('CreateCollectionModal.LabelRequired'));
            return;
        }

        setLoading(true);
        setError(null);

        try {
            await onSubmit(label);
            setLabel('');
            onClose();
        } catch (err) {
            setError(err.error || err.message || t('CreateCollectionModal.Error') || 'Erreur lors de la création');
            console.error('Erreur création collection:', err);
        } finally {
            setLoading(false);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content glass" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <h2>{t('CreateCollectionModal.Title')}</h2>
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
                        <input type="text"
                               name="collection-labe"
                               id="collection-label"
                               value={label}
                               onChange={(e) => setLabel(e.target.value)}
                               placeholder={t('CreateCollectionModal.Placeholder')}
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
                            {t('CreateCollectionModal.Cancel')}
                        </button>
                        <button
                            type="submit"
                            disabled={loading}
                            className="btn-primary"
                        >
                            {loading ? (t('CreateCollectionModal.Creating')) : (t('CreateCollectionModal.Create'))}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}