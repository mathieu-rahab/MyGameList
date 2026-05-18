import { StrictMode, Suspense } from 'react'
import { createRoot } from 'react-dom/client'
import { CookiesProvider } from 'react-cookie';
import App from './App.jsx'
import './global.css'
import '@tabler/icons-webfont/dist/tabler-icons.css';
import './utils/i18n.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <CookiesProvider>
      <Suspense fallback={<div>Chargement des traductions...</div>}>
        <App />
      </Suspense>
    </CookiesProvider>
  </StrictMode>,
)
