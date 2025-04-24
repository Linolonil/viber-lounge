import { Router } from 'express';
import { ProdutoController } from '../controllers/ProdutoController';
import { validateProduto } from '../middlewares/produtoValidation';
import multer from 'multer';

const router = Router();
const produtoController = new ProdutoController();


const storage = multer.diskStorage({
    destination: "uploads",
    filename:(req, file, cb)=>{
        return cb(null, `${Date.now()}${file.originalname}`)
    }
})
const upload = multer({storage:storage})


router.get('/produtos', (req, res) => produtoController.getAll(req, res));
router.get('/produtos/:id', (req, res) => produtoController.getById(req, res));
router.post('/produtos',  upload.single("image"), validateProduto, (req, res, next) => produtoController.create(req, res, next));
router.put('/produtos/:id', upload.single('imagem'), validateProduto,  (req, res, next) => produtoController.update(req, res, next));
router.delete('/produtos/:id', (req, res) => produtoController.delete(req, res));

export default router; 