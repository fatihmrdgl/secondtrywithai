import { useMemo, useState } from 'react'
import AuthPanel from './components/AuthPanel'
import CustomersPanel from './components/CustomersPanel'
import ProductsPanel from './components/ProductsPanel'

const initialAuth = JSON.parse(localStorage.getItem('mimcrm-auth') || 'null')

export default function App() {
  const [auth, setAuth] = useState(initialAuth)

  const welcomeText = useMemo(() => (
    auth
      ? `Hoş geldin ${auth.fullName}, mimcrm paneline giriş yaptın.`
      : 'mimcrm sigorta acente yönetim paneline hoş geldiniz.'
  ), [auth])

  return (
    <main className="container">
      <header className="hero card">
        <h1>mimcrm</h1>
        <p>{welcomeText}</p>
      </header>

      {!auth ? (
        <AuthPanel onAuthSuccess={setAuth} />
      ) : (
        <>
          <section className="card">
            <h3>Kullanıcı Yönetimi</h3>
            <p>Aktif kullanıcı: {auth.fullName} ({auth.email})</p>
            <button onClick={() => { localStorage.removeItem('mimcrm-auth'); setAuth(null) }}>
              Güvenli Çıkış
            </button>
          </section>
          <CustomersPanel />
          <ProductsPanel />
        </>
      )}
    </main>
  )
}
