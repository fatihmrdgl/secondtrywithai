import axios from 'axios'

const identityApi = axios.create({ baseURL: 'http://localhost:5001' })
const customerApi = axios.create({ baseURL: 'http://localhost:5002' })
const productApi = axios.create({ baseURL: 'http://localhost:5003' })

export { identityApi, customerApi, productApi }
