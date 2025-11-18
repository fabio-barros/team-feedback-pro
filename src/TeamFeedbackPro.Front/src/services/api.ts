import axios from 'axios';

const api = axios.create({
  
  baseURL: // 'http://localhost:3000/api' url da api
  'https://team-feedback-pro-backend.onrender.com/api'
});


export default api;