export const useApi = () => {
  const { apiBase } = useRuntimeConfig().public;
  return $fetch.create({ baseURL: apiBase as string });
};
