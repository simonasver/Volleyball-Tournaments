import api from "./api";

export const getUser = async (id: string, signal?: AbortSignal) => {
  const res = await api.get("/Users/" + id, { signal: signal });
  return res.data;
};

export const editUser = async (
  id: string,
  profilePictureUrl = "",
  fullName = "",
  signal?: AbortSignal
) => {
  const res = await api.patch(
    "/Users/" + id,
    {
      ProfilePictureUrl: profilePictureUrl,
      FullName: fullName,
    },
    { signal: signal }
  );
  return res.data;
};
