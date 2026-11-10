import { reactive } from 'vue';
import { resources } from './api.js';

export const resourceState = reactive({ data: null, status: 'loading', error: '' });
let pending;
export function loadResources() {
  if (pending) return pending;
  pending = resources.list().then(data => {
    if (!data || !Array.isArray(data.readings)) throw new Error('Invalid resource response.');
    resourceState.data = data;
    resourceState.status = 'ready';
    return data;
  }).catch(error => {
    resourceState.status = 'offline';
    resourceState.error = error?.message || 'Resources are temporarily unavailable.';
    return null;
  }).finally(() => { pending = null; });
  return pending;
}
