import { useEffect, useState } from 'react'
import { customerApi } from '../api/client'

const emptyCustomer = { fullName: '', phone: '', email: '', city: '' }

export default function CustomersPanel() {
  const [customers, setCustomers] = useState([])
  const [form, setForm] = useState(emptyCustomer)

  const load = async () => {
    const { data } = await customerApi.get('/api/customers')
    setCustomers(data)
  }

  useEffect(() => {
    load()
  }, [])

  const addCustomer = async (event) => {
    event.preventDefault()
    await customerApi.post('/api/customers', form)
    setForm(emptyCustomer)
    load()
  }

  return (
    <section className="card">
      <h3>Müşteri Yönetimi</h3>
      <form className="grid" onSubmit={addCustomer}>
        <input placeholder="Ad Soyad" value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} required />
        <input placeholder="Telefon" value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} required />
        <input type="email" placeholder="E-posta" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} required />
        <input placeholder="Şehir" value={form.city} onChange={(e) => setForm({ ...form, city: e.target.value })} required />
        <button type="submit">Müşteri Ekle</button>
      </form>

      <ul>
        {customers.map((customer) => (
          <li key={customer.id}>{customer.fullName} - {customer.phone} - {customer.city}</li>
        ))}
      </ul>
    </section>
  )
}
