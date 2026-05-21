/**
 * Traduit un code d'erreur serveur en message localisé.
 * @param {string} errorCode - Le code retourné par l'API (ex: "EMAIL_ALREADY_EXISTS")
 * @param {Function} t - Fonction de traduction i18next
 * @param {Function} i18n - Instance i18next (pour i18n.exists)
 * @param {string} namespace - Namespace i18n de la page (ex: "CreateAccount")
 */

const dummyForExtractor = (t) => {
    // Erreurs de création de compte
    t('CreateAccount.ServerErrors.EMAIL_ALREADY_EXISTS');
    t('CreateAccount.ServerErrors.INVALID_PSEUDO');
    t('CreateAccount.ServerErrors.PSEUDO_ALREADY_EXIST');
    t('CreateAccount.ServerErrors.UNKNOWN');
};


export function getServerErrorMessage(errorCode, t, i18n, namespace) {
    const key = `${namespace}.ServerErrors.${errorCode}`;
    return i18n.exists(key) ? t(key) : getHttpErrorMessage(null, t);
}

/**
 * Traduit un status HTTP en message localisé.
 * @param {number|null} status - Le status HTTP (ex: 502)
 * @param {Function} t - Fonction de traduction i18next
 */
export function getHttpErrorMessage(status, t) {
    switch (status) {
        case 502: return t('NetworkErrors.BAD_GATEWAY');
        case 500: return t('NetworkErrors.INTERNAL_SERVER_ERROR');
        default:  return t('NetworkErrors.SERVER_UNREACHABLE');
    }
}