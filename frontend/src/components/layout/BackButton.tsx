import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { Button } from '@mui/material';
import { useNavigate } from 'react-router-dom';

interface BackButtonProps {
    title?: string;
    address?: string;
}

const BackButton = (props: BackButtonProps) => {
    const { title, address } = props;

    const navigate = useNavigate();
    return <Button variant="outlined" startIcon={<ArrowBackIcon />} onClick={() => address ? navigate(address) : navigate(-1)}>{title ? title : "Back"}</Button>;
};

export default BackButton;