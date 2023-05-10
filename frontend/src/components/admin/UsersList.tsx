import {
  TableContainer,
  Paper,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  Grid,
  Pagination,
  Button,
} from "@mui/material";
import React from "react";
import BackButton from "../layout/BackButton";
import { useNavigate, useSearchParams } from "react-router-dom";
import {
  getDefaultPageSize,
  formatPaginationDataToQuery,
  getDefaultPaginationData,
  errorMessageFromAxiosError,
} from "../../utils/helpers";
import { PageData } from "../../utils/types";
import { User, UserRole } from "../../store/auth-slice";
import { useAppDispatch } from "../../utils/hooks";
import { alertActions } from "../../store/alert-slice";
import Loader from "../layout/Loader";
import { getUsers, ban, addRemoveRole } from "../../services/user.service";
import AddRoleModal from "./AddRoleModal";
import RemoveRoleModal from "./RemoveRoleModal";
import SearchFilterInput from "../layout/SearchFilterInput";

enum Modal {
  None = 0,
  AddRole = 1,
  RemoveRole = 2,
}

const UsersList = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const [searchInput, setSearchInput] = React.useState("");

  const [modalStatus, setModalStatus] = React.useState(Modal.None);

  const [addRoleHeader, setAddRoleHeader] = React.useState("");
  const [removeRoleHeader, setRemoveRoleHeader] = React.useState("");

  const [selectedUser, setSelectedUser] = React.useState("");

  const [isLoading, setIsLoading] = React.useState(true);

  const [users, setUsers] = React.useState<User[]>();

  const [query, setQuery] = useSearchParams();
  const [currentPage, setCurrentPage] = React.useState<{
    pageNumber: number;
    pageSize: number;
  }>({ pageNumber: 1, pageSize: getDefaultPageSize() });
  const [pagination, setPagination] = React.useState<PageData>();

  const setSearchParams = (pageNumber: number, pageSize: number) => {
    query.set("pageNumber", pageNumber.toString());
    query.set("pageSize", pageSize.toString());
    setQuery(query, { replace: true });
    setCurrentPage({ pageNumber, pageSize });
  };

  React.useEffect(() => {
    const pageNumber: number =
      parseInt(query.get("pageNumber") ?? "") ?? getDefaultPageSize();
    const pageSize: number = parseInt(query.get("pageSize") ?? "") ?? 1;
    if (!pageNumber && !pageSize) {
      navigate(
        `/users?${formatPaginationDataToQuery(getDefaultPaginationData())}`,
        { replace: true }
      );
    } else {
      setSearchParams(pageNumber, pageSize);
    }
  }, [query]);

  React.useEffect(() => {
    const abortController = new AbortController();
    getUsers(currentPage.pageNumber, currentPage.pageSize, searchInput)
      .then((res) => {
        setUsers(res.data);
        setPagination(res.pagination);
        setIsLoading(false);
      })
      .catch((e) => {
        console.log(e);
        dispatch(
          alertActions.changeAlert({
            type: "error",
            message: errorMessageFromAxiosError(e),
          })
        );
        setIsLoading(false);
      });
    return () => abortController.abort();
  }, [currentPage, searchInput]);

  const changeBanState = (userId: string, userName: string, state: boolean) => {
    ban(userId, state)
      .then(() => {
        dispatch(
          alertActions.changeAlert({
            type: "success",
            message: `User ${userName} was successfully ${
              state ? "banned" : "unbanned"
            }`,
          })
        );
        getUsers(currentPage.pageNumber, currentPage.pageSize, searchInput)
          .then((res) => {
            setUsers(res.data);
            setPagination(res.pagination);
          })
          .catch((e) => {
            console.log(e);
            dispatch(
              alertActions.changeAlert({
                type: "error",
                message: errorMessageFromAxiosError(e),
              })
            );
          });
      })
      .catch((e) => {
        console.log(e);
        dispatch(
          alertActions.changeAlert({
            type: "error",
            message: errorMessageFromAxiosError(e),
          })
        );
      });
  };

  const closeModals = () => {
    setModalStatus(Modal.None);
    setSelectedUser("");
  };

  const onAddRemoveRole = (role: UserRole, action: boolean) => {
    const userId = selectedUser;
    addRemoveRole(userId, role, action)
      .then(() => {
        dispatch(
          alertActions.changeAlert({
            type: "success",
            message: `User ${
              users?.find((x) => x.id === userId)?.userName
            } successfully had ${role} role ${action ? "added" : "removed"}`,
          })
        );
        closeModals();
        getUsers(currentPage.pageNumber, currentPage.pageSize, searchInput)
          .then((res) => {
            setUsers(res.data);
            setPagination(res.pagination);
            setIsLoading(false);
          })
          .catch((e) => {
            console.log(e);
            dispatch(
              alertActions.changeAlert({
                type: "error",
                message: errorMessageFromAxiosError(e),
              })
            );
            setIsLoading(false);
          });
      })
      .catch((e) => {
        console.log(e);
        dispatch(
          alertActions.changeAlert({
            type: "error",
            message: errorMessageFromAxiosError(e),
          })
        );
      });
  };

  const openAddRoleModal = (userId: string) => {
    setModalStatus(Modal.AddRole);
    setSelectedUser(userId);
    setAddRoleHeader(
      `Add a role to the user ${users?.find((x) => x.id === userId)?.userName}`
    );
  };

  const openRemoveRoleModal = (userId: string) => {
    setModalStatus(Modal.RemoveRole);
    setSelectedUser(userId);
    setRemoveRoleHeader(
      `Remove a role from the user ${
        users?.find((x) => x.id === userId)?.userName
      }`
    );
  };

  return (
    <>
      <Grid item sx={{ width: "100%" }}>
        <Loader isOpen={isLoading} />
        <Grid
          container
          spacing={1}
          direction="row"
          alignItems="center"
          justifyContent="space-between"
        >
          <Grid item>
            <BackButton title="Admin panel" address={`/admin`} />
          </Grid>
          <Grid item>
            <SearchFilterInput
              label="Search by username"
              searchInput={searchInput}
              onSearchInputChange={setSearchInput}
            />
          </Grid>
        </Grid>
        <br />
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>#</TableCell>
                <TableCell>Username</TableCell>
                <TableCell>Fullname</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Register date</TableCell>
                <TableCell>Last login date</TableCell>
                <TableCell>Roles</TableCell>
                <TableCell>Add role</TableCell>
                <TableCell>Remove role</TableCell>
                <TableCell>Block</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {users?.map((user, index) => (
                <TableRow key={user.id}>
                  <TableCell>{index + 1}</TableCell>
                  <TableCell>{user.userName}</TableCell>
                  <TableCell>{user.fullName}</TableCell>
                  <TableCell>{user.email}</TableCell>
                  <TableCell>
                    {user.registerDate &&
                      new Date(user.registerDate).toLocaleString()}
                  </TableCell>
                  <TableCell>
                    {user.lastLoginDate &&
                      new Date(user.lastLoginDate).toLocaleString()}
                  </TableCell>
                  <TableCell>{user.roles.join(", ")}</TableCell>
                  <TableCell>
                    <Button
                      variant="contained"
                      size="small"
                      color="primary"
                      onClick={() => openAddRoleModal(user.id)}
                    >
                      Add
                    </Button>
                  </TableCell>
                  <TableCell>
                    <Button
                      variant="contained"
                      size="small"
                      color="secondary"
                      onClick={() => openRemoveRoleModal(user.id)}
                    >
                      Remove
                    </Button>
                  </TableCell>
                  <TableCell>
                    {user.banned && (
                      <Button
                        variant="contained"
                        size="small"
                        color="success"
                        onClick={() =>
                          changeBanState(user.id, user.userName, false)
                        }
                      >
                        Unban
                      </Button>
                    )}
                    {!user.banned && (
                      <Button
                        variant="contained"
                        size="small"
                        color="error"
                        onClick={() =>
                          changeBanState(user.id, user.userName, true)
                        }
                      >
                        Ban
                      </Button>
                    )}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
        <br />
        <Grid
          container
          spacing={1}
          direction="row"
          alignItems="center"
          justifyContent="center"
        >
          <Grid item>
            {(users?.length ?? 0) > 0 && (
              <Pagination
                defaultPage={currentPage.pageNumber}
                count={pagination?.totalPages}
                onChange={(event: React.ChangeEvent<unknown>, page: number) =>
                  setSearchParams(page, currentPage.pageSize)
                }
                color="primary"
              />
            )}
          </Grid>
        </Grid>
      </Grid>
      {modalStatus === Modal.AddRole && (
        <AddRoleModal
          header={addRoleHeader}
          onSubmit={onAddRemoveRole}
          onClose={closeModals}
        />
      )}
      {modalStatus === Modal.RemoveRole && (
        <RemoveRoleModal
          header={removeRoleHeader}
          onSubmit={onAddRemoveRole}
          onClose={closeModals}
        />
      )}
    </>
  );
};

export default UsersList;
