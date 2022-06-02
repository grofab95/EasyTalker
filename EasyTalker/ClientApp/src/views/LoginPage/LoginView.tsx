import styles from './LoginView.module.css'

import React from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { ApplicationState } from '../../store'
import { useHistory } from 'react-router-dom'
import * as Yup from 'yup'
import { useFormik } from 'formik'
import { login } from '../../store/userSession/api'
import { Button, Col, Form, Modal, Spinner } from 'react-bootstrap'
import Logo from '../../app/dashboard/Logo'
import RegisterView from '../RegisterPage/RegisterView'

const LoginView: React.FC = () => {
    const anyBusy = useSelector((state: ApplicationState) => state.userSession.isBusy)
    
    const dispatch = useDispatch()
    const token = useSelector((state: ApplicationState) => state.userSession.currentToken)
    const history = useHistory()

    const [showRegisterModal, setShowRegisterModal] = React.useState(false)
        
    React.useEffect(() => {
        if (token !== '') {
            history.replace('/')
        }
    }, [history, token])

    const getInitialValues = () => {
        return {
            username: '',
            password: ''
        }
    }

    const getLoginValidationSchema = () => Yup.object().shape({
        username: Yup.string().trim().required('Username is required'),
        password: Yup.string().trim().required('Password is required')
    })

    const formik = useFormik({
        initialValues: getInitialValues(),
        validationSchema: getLoginValidationSchema(),
        validateOnBlur: false,
        validateOnChange: false,
        onSubmit: (values) => {
            dispatch(login({
                username: values.username,
                password: values.password
            }))
        }
    })
        
    return <>
        <Modal show={anyBusy} centered={true}>
            <Modal.Body className="text-center">
                <h2>Please Wait</h2>
                <Spinner animation="border" variant="primary" />
            </Modal.Body>
        </Modal>
        <Modal show={showRegisterModal} onHide={() => setShowRegisterModal(false)} centered>
            <Modal.Header closeButton>
                <Modal.Title>
                    Sign Up
                </Modal.Title>
            </Modal.Header>            
            <Modal.Body>
                <RegisterView onSuccessfulRegister={() => setShowRegisterModal(false)} />
            </Modal.Body>
            <Modal.Footer />
        </Modal>
        
        <div className={styles.formBody}>
            <Form className={`row g-3 ${styles.formRow}`} onSubmit={formik.handleSubmit} id="loginForm">
                <Col lg="12" className="d-flex align-items-center justify-content-center mb-5">
                    <Logo isNav={false} />
                </Col>
                <Col lg="12">
                    <Form.Group>
                        <Form.Label>Username</Form.Label>
                        <Form.Control type="text"
                                      id="username"
                                      value={formik.values.username}
                                      isInvalid={!!formik.errors.username}
                                      placeholder="Enter username"
                                      onChange={formik.handleChange}
                        />
                        <Form.Control.Feedback type="invalid">
                            {formik.errors.username}
                        </Form.Control.Feedback>
                    </Form.Group>
                    <Form.Group>
                        
                    </Form.Group>
                </Col>

                <Col lg="12">
                    <Form.Label>Password</Form.Label>
                    <Form.Control type="password"
                                  id="password"
                                  value={formik.values.password}
                                  isInvalid={!!formik.errors.password}
                                  placeholder="Enter password"
                                  onChange={formik.handleChange}
                    />
                    <Form.Control.Feedback type="invalid">
                        {formik.errors.password}
                    </Form.Control.Feedback>
                </Col>

                <Col lg="12">
                    <div className="d-grid">
                        <Button variant="success" type="submit">
                            <i className="bs bxs-lock-green"/>Sign in
                        </Button>
                    </div>
                </Col>

                <Col lg="12" className="d-flex justify-content-center">
                    Don&apos;t have an account yet? <span className="ms-1" style={{color: 'green'}}><p onClick={() => setShowRegisterModal(true)}>Sign Up!</p></span>
                </Col>
            </Form>
        </div>
    </>
}
export default LoginView