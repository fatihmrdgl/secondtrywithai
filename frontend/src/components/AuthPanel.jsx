import { useState } from 'react'
import { identityApi } from '../api/client'

export default function AuthPanel({ onAuthSuccess }) {
  const [mode, setMode] = useState('login')
  const [form, setForm] = useState({ fullName: '', email: '', password: '' })
  const [message, setMessage] = useState('')

  const submit = async (event) => {
    event.preventDefault()
    setMessage('')
    try {
      const endpoint = mode === 'login' ? '/api/auth/login' : '/api/auth/register'
      const payload = mode === 'login'
        ? { email: form.email, password: form.password }
        : { fullName: form.fullName, email: form.email, password: form.password }

      const { data } = await identityApi.post(endpoint, payload)
      localStorage.setItem('mimcrm-auth', JSON.stringify(data))
      onAuthSuccess(data)
      setMessage('Giriş başarılı!')
    } catch (error) {
      setMessage(error.response?.data?.message ?? 'İşlem sırasında hata oluştu.')
    }
  }

  return (
    <section className="card auth-card">
      <h2>{mode === 'login' ? 'Giriş Yap' : 'Yeni Kullanıcı Kaydı'}</h2>
      <form onSubmit={submit}>
        {mode === 'register' && (
          <input
            placeholder="Ad Soyad"
            value={form.fullName}
            onChange={(e) => setForm({ ...form, fullName: e.target.value })}
            required
          />
        )}
        <input
          type="email"
          placeholder="E-posta"
          value={form.email}
          onChange={(e) => setForm({ ...form, email: e.target.value })}
          required
        />
        <input
          type="password"
          placeholder="Parola"
          value={form.password}
          onChange={(e) => setForm({ ...form, password: e.target.value })}
          required
        />
        <button type="submit">{mode === 'login' ? 'Giriş Yap' : 'Kayıt Ol'}</button>
      </form>
      <button className="link" onClick={() => setMode(mode === 'login' ? 'register' : 'login')}>
        {mode === 'login' ? 'Yeni kullanıcı oluştur' : 'Hesabım var, giriş yap'}
      </button>
      {message && <p className="message">{message}</p>}
    </section>
  )
}
