import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import Backend from 'i18next-http-backend';
import LanguageDetector from 'i18next-browser-languagedetector';


i18n
  .use(Backend) // Indique à i18next d'utiliser le chargeur HTTP
  .use(LanguageDetector)
  .use(initReactI18next) // Passe l'instance i18n à react-i18next
  .init({
    
    fallbackLng: 'fr', // Langue par défaut si une trad manque
    
    backend: {
      loadPath: '/locales/{{lng}}/{{ns}}.json',
    },

    detection: {
      order: ['localStorage', 'navigator'], // ordre de source du choix de langue
      lookupLocalStorage: 'i18nextLng',
      caches: ['localStorage'], // stockage de la langue favorite 
    },

    interpolation: {
      escapeValue: false, 
    }
  });

export default i18n;