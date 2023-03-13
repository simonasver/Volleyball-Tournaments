import React from "react";

import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Container from "@mui/material/Container";
import IconButton from "@mui/material/IconButton";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import MenuIcon from "@mui/icons-material/Menu";
import SportsVolleyballIcon from "@mui/icons-material/SportsVolleyball";
import { Avatar, Button, Menu, MenuItem, Tooltip } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useAppSelector } from "../../hooks";

const Navbar = () => {
  const navigate = useNavigate();

  const user = useAppSelector((state) => state.auth.user);

  const title = "Volleyball";

  const navMenuItems = [
    { title: "Home", href: "/" },
    { title: "Help", href: "/asdf" },
  ];
  const profileMenuItems = [
    { title: "Profile", href: "/profile" },
    { title: "Logout", href: "/logout" },
  ];

  const [anchorElNav, setAnchorElNav] = React.useState<null | HTMLElement>(
    null
  );
  const [anchorElUser, setAnchorElUser] = React.useState<null | HTMLElement>(
    null
  );

  const handleOpenNavMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorElNav(event.currentTarget);
  };
  const handleOpenUserMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorElUser(event.currentTarget);
  };

  const handleNavMenuClick = (href?: string) => {
    if (href) {
      navigate(href);
    }
    setAnchorElNav(null);
  };

  const handleUserMenuClick = (href?: string) => {
    if (href) {
      navigate(href);
    }
    setAnchorElUser(null);
  };

  return (
    <>
      <AppBar sx={{ boxShadow: 0 }}>
        <Container maxWidth="xl">
          <Toolbar disableGutters>
            <SportsVolleyballIcon
              sx={{ display: { xs: "none", md: "flex" }, mr: 1 }}
            />
            <Typography
              variant="h6"
              noWrap
              component="a"
              href="/"
              sx={{
                mr: 2,
                display: { xs: "none", md: "flex" },
                fontFamily: "monospace",
                fontWeight: 700,
                letterSpacing: ".3rem",
                color: "inherit",
                textDecoration: "none",
              }}
            >
              {title}
            </Typography>

            <Box sx={{ flexGrow: 1, display: { xs: "flex", md: "none" } }}>
              <IconButton
                size="large"
                aria-label="account of current user"
                aria-controls="menu-appbar"
                aria-haspopup="true"
                onClick={handleOpenNavMenu}
                color="inherit"
              >
                <MenuIcon />
              </IconButton>
              <Menu
                id="menu-appbar"
                anchorEl={anchorElNav}
                anchorOrigin={{ vertical: "bottom", horizontal: "left" }}
                keepMounted
                transformOrigin={{ vertical: "top", horizontal: "left" }}
                open={Boolean(anchorElNav)}
                onClose={() => handleNavMenuClick()}
                sx={{ display: { xs: "block", md: "none" } }}
              >
                {navMenuItems.map((item) => (
                  <MenuItem
                    key={item.title}
                    onClick={() => handleNavMenuClick(item.href)}
                  >
                    <Typography textAlign="center">{item.title}</Typography>
                  </MenuItem>
                ))}
              </Menu>
            </Box>
            <SportsVolleyballIcon
              sx={{ display: { xs: "flex", md: "none" }, mr: 1 }}
            />
            <Typography
              variant="h5"
              noWrap
              component="a"
              href=""
              sx={{
                mr: 2,
                display: { xs: "flex", md: "none" },
                flexGrow: 1,
                fontFamily: "monospace",
                fontWeight: 700,
                letterSpacing: ".3rem",
                color: "inherit",
                textDecoration: "none",
              }}
            >
              {title}
            </Typography>
            <Box sx={{ flexGrow: 1, display: { xs: "none", md: "flex" } }}>
              {navMenuItems.map((item) => (
                <Button
                  key={item.title}
                  onClick={() => handleNavMenuClick(item.href)}
                  sx={{ my: 2, color: "white", display: "block" }}
                >
                  {item.title}
                </Button>
              ))}
            </Box>
            {user && (
              <Box sx={{ flexGrow: 0 }}>
                <Tooltip title={user.fullName}>
                  <IconButton onClick={handleOpenUserMenu} sx={{ p: 0 }}>
                    <Avatar alt={user.fullName} src={user.profilePictureUrl} />
                  </IconButton>
                </Tooltip>
                <Menu
                  sx={{ mt: "45px" }}
                  id="menu-appbar"
                  anchorEl={anchorElUser}
                  anchorOrigin={{ vertical: "top", horizontal: "right" }}
                  keepMounted
                  transformOrigin={{ vertical: "top", horizontal: "right" }}
                  open={Boolean(anchorElUser)}
                  onClose={() => handleUserMenuClick()}
                >
                  {profileMenuItems.map((item) => (
                    <MenuItem
                      key={item.title}
                      onClick={() => handleUserMenuClick(item.href)}
                    >
                      <Typography textAlign="center">{item.title}</Typography>
                    </MenuItem>
                  ))}
                </Menu>
              </Box>
            )}
            {!user && (
              <Box sx={{ flexGrow: 0, flexDirection: "row" }}>
                <Button
                  key="login"
                  sx={{ my: 2, color: "white" }}
                  onClick={() => navigate("/login")}
                >
                  Login
                </Button>
                <Button
                  key="register"
                  sx={{ my: 2, color: "white", display: { xs: "none", md: "inline-flex" } }}
                  onClick={() => navigate("/register")}
                >
                  Register
                </Button>
              </Box>
            )}
          </Toolbar>
        </Container>
      </AppBar>
      <Toolbar />
    </>
  );
};

export default Navbar;
