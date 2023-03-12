import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { Button } from '@mui/material';
import { useNavigate } from 'react-router-dom';

const BackButton = () => {
    const navigate = useNavigate();
    return <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(-1)}>Back</Button>;
};

export default BackButton;