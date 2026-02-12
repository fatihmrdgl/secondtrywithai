import { useEffect, useState } from 'react'
import { productApi } from '../api/client'

const emptyProduct = { name: '', category: '', price: 0, description: '' }

export default function ProductsPanel() {
  const [products, setProducts] = useState([])
  const [form, setForm] = useState(emptyProduct)

  const load = async () => {
    const { data } = await productApi.get('/api/products')
    setProducts(data)
  }

  useEffect(() => {
    load()
  }, [])

  const addProduct = async (event) => {
    event.preventDefault()
    await productApi.post('/api/products', { ...form, price: Number(form.price) })
    setForm(emptyProduct)
    load()
  }

  return (
    <section className="card">
      <h3>Ürün (Poliçe) Yönetimi</h3>
      <form className="grid" onSubmit={addProduct}>
        <input placeholder="Ürün Adı" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
        <input placeholder="Kategori" value={form.category} onChange={(e) => setForm({ ...form, category: e.target.value })} required />
        <input type="number" min="0" step="0.01" placeholder="Fiyat" value={form.price} onChange={(e) => setForm({ ...form, price: e.target.value })} required />
        <input placeholder="Açıklama" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} required />
        <button type="submit">Ürün Ekle</button>
      </form>

      <ul>
        {products.map((product) => (
          <li key={product.id}>{product.name} - {product.category} - {product.price} ₺</li>
        ))}
      </ul>
    </section>
  )
}
