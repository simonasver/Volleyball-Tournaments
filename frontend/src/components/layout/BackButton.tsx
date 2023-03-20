import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { Button } from '@mui/material';
import { useNavigate } from 'react-router-dom';

interface BackButtonProps {
    address: string;
    title?: string;
}

const BackButton = (props: BackButtonProps) => {
    const { title, address } = props;

    const navigate = useNavigate();
    return <Button variant="outlined" startIcon={<ArrowBackIcon />} onClick={() => navigate(address)}>{title ? title : "Back"}</Button>;
};

export default BackButton;