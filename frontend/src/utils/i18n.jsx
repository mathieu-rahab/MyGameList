import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import Backend from 'i18next-http-backend';

i18n
  .use(Backend) // Indique à i18next d'utiliser le chargeur HTTP
  .use(initReactI18next) // Passe l'instance i18n à react-i18next
  .init({
    fallbackLng: 'fr', // Langue par défaut si une trad manque
    lng: 'fr', // Langue de départ
    
    // C'est ici que la magie opère : le chemin vers tes JSON
    backend: {
      loadPath: '/locales/{{lng}}/{{ns}}.json',
    },

    interpolation: {
      escapeValue: false, // React protège déjà contre les failles XSS
    }
  });

export default i18n;