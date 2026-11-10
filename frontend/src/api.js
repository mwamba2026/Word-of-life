import { apiBaseUrl } from './config.js';

const OFFLINE_MESSAGE = 'We could not reach the Words of Life server. Please try again a little later.';
const USER_KEY = 'wol.user';
async function csrfToken() {
  let response;
  try { response = await fetch(`${apiBaseUrl}/api/security/csrf`, { credentials: 'include', cache: 'no-store' }); }
  catch { throw new Error(OFFLINE_MESSAGE); }
  if (!response.ok) throw new Error('Could not start a secure request. Refresh and try again.');
  const data = await response.json();
  return data.token;
}

async function request(path, { method = 'GET', body, headers = {} } = {}) {
  let response;
  try {
    const csrf = method === 'GET' ? null : await csrfToken();
    response = await fetch(`${apiBaseUrl}${path}`, {
      method,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', ...(csrf ? { 'X-CSRF-TOKEN': csrf } : {}), ...headers },
      body: body === undefined ? undefined : JSON.stringify(body)
    });
  } catch {
    throw new Error(OFFLINE_MESSAGE);
  }

  const data = await response.json().catch(() => null);
  if (!response.ok) {
    throw new Error(errorMessage(data, response.status));
  }
  return data;
}

// The API sends { message } or { errors: [...] }; ASP.NET validation sends { title, errors: { field: [...] } }.
function errorMessage(data, status) {
  if (data?.message) return data.message;
  const errors = data?.errors;
  if (Array.isArray(errors) && errors.length) return errors.join(' ');
  if (errors && typeof errors === 'object') {
    const list = Object.values(errors).flat().filter(Boolean);
    if (list.length) return list.join(' ');
  }
  return data?.detail || data?.title || `Request failed (${status}).`;
}

// crypto.randomUUID only exists on secure pages (https or localhost), not on a phone using http://192.168.x.x.
function newId() {
  if (globalThis.crypto?.randomUUID) return globalThis.crypto.randomUUID();
  return `${Date.now().toString(36)}-${Math.random().toString(36).slice(2, 10)}`;
}

function readJson(key, fallback) {
  try {
    return JSON.parse(localStorage.getItem(key)) ?? fallback;
  } catch {
    return fallback;
  }
}

function writeJson(key, value) {
  try {
    localStorage.setItem(key, JSON.stringify(value));
  } catch {
    // Storage can be unavailable (private mode); the page still works for this visit.
  }
}

export const auth = {
  async register({ fullName, email, password, inviteCode }) {
    return request('/api/auth/register', { method: 'POST', body: { fullName, email, password, inviteCode } });
  },

  async login({ email, password }) {
    const data = await request('/api/auth/login', { method: 'POST', body: { email, password } });
    writeJson(USER_KEY, data.user);
    return data.user;
  },

  async logout() {
    try { await request('/api/auth/logout', { method: 'POST' }); } catch {}
    try { localStorage.removeItem(USER_KEY); } catch {}
  },

  bootstrapAdmin(key) {
    return request('/api/admin/bootstrap', { method: 'POST', headers: { 'X-Admin-Bootstrap-Key': key } });
  },

  async me() {
    try {
      const data = await request('/api/auth/me');
      writeJson(USER_KEY, data.user);
      return data.user;
    } catch (error) {
      try { localStorage.removeItem(USER_KEY); } catch {}
      throw error;
    }
  },

  currentUser() {
    return readJson(USER_KEY, null);
  }
};

export const notes = {
  list() { return request('/api/notes'); },
  save(note) {
    return note.id
      ? request(`/api/notes/${note.id}`, { method: 'PUT', body: note })
      : request('/api/notes', { method: 'POST', body: note });
  },
  remove(id) { return request(`/api/notes/${id}`, { method: 'DELETE' }); }
};

// Support and feedback are stored by the .NET API.
export const messages = {
  sendFeedback({ name, email, message }) {
    return request('/api/feedback', { method: 'POST', body: { name, email, message } });
  },

  sendSupportRequest({ name, email, phone, topic, message }) {
    return request('/api/support', { method: 'POST', body: { name, email, phone, topic, message } });
  }
};

export const content = {
  list() { return request('/api/content'); },
  listAdmin() { return request('/api/content/admin'); },
  create(item) { return request('/api/content', { method: 'POST', body: item }); },
  update(id, item) { return request(`/api/content/${id}`, { method: 'PUT', body: item }); },
  remove(id) { return request(`/api/content/${id}`, { method: 'DELETE' }); }
};

export const media = {
  async upload(file) {
    const body = new FormData();
    body.append('file', file);
    let response;
    try {
      const csrf = await csrfToken();
      response = await fetch(`${apiBaseUrl}/api/media`, {
        method: 'POST', credentials: 'include', headers: { 'X-CSRF-TOKEN': csrf }, body
      });
    }
    catch { throw new Error(OFFLINE_MESSAGE); }
    const data = await response.json().catch(() => null);
    if (!response.ok) throw new Error(errorMessage(data, response.status));
    return data;
  }
};

export const resources = {
  list() { return request('/api/resources'); }
};
