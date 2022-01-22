// @ts-ignore
import styles from './RegisterView.module.css'
import React from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { ApplicationState } from '../../store'
import { useHistory } from 'react-router-dom'
import * as Yup from 'yup'
import { useFormik } from 'formik'
import { login } from '../../store/userSession/api'
import { Button, Col, Form, Modal, Spinner } from 'react-bootstrap'
import Logo from '../../app/dashboard/Logo'
import { registerUser } from '../../store/users/api'
import RegisterUser from '../../interfaces/Users/RegisterUser'

const RegisterView: React.FC = () => {
    const dispatch = useDispatch()
    const anyBusy = useSelector((state: ApplicationState) => state.user.isBusy)
    // const token = useSelector((state: ApplicationState) => state.userSession.currentToken)
    const history = useHistory()
    //
    // React.useEffect(() => {
    //     if (token !== '') {
    //         history.replace('/app')
    //     }
    // }, [history, token])

    const getInitialValues = () => {
        return {
            username: '',
            email: '',
            password: '',
            passwordConfirmation: ''
        }
    }

    const getRegisterValidationSchema = () => Yup.object().shape({
        username: Yup.string().required('Username is required'),
        email: Yup.string().required('Email is required').email('Email format is invalid'),
        password: Yup.string().required('Password is required'),
        passwordConfirmation: Yup.string().oneOf([Yup.ref('password'), null], 'Passwords must match').required('Confirm password is required')
    })

    const formik = useFormik({
        initialValues: getInitialValues(),
        validationSchema: getRegisterValidationSchema(),
        validateOnBlur: false,
        validateOnChange: false,
        onSubmit: (values) => {
            dispatch(registerUser({ 
                    registeredUser: {
                        userName: values.username,
                        email: values.email,
                        password: values.password
                    } as RegisterUser,
                    onSuccessfulResponse: () => history.replace('/')               
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
        <div className={styles.formBody}>
            <Form className={`row g-3 ${styles.formRow}`} onSubmit={formik.handleSubmit}>
                <Col lg="12">
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
                </Col>

                <Col lg="12">
                    <Form.Label>Email</Form.Label>
                    <Form.Control type="text"
                                  id="email"
                                  value={formik.values.email}
                                  isInvalid={!!formik.errors.email}
                                  placeholder="Enter email"
                                  onChange={formik.handleChange}
                    />
                    <Form.Control.Feedback type="invalid">
                        {formik.errors.email}
                    </Form.Control.Feedback>
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
                    <Form.Label>Confirm Password</Form.Label>
                    <Form.Control type="password"
                                  id="passwordConfirmation"
                                  value={formik.values.passwordConfirmation}
                                  isInvalid={!!formik.errors.passwordConfirmation}
                                  placeholder="Confirm password"
                                  onChange={formik.handleChange}
                    />
                    <Form.Control.Feedback type="invalid">
                        {formik.errors.passwordConfirmation}
                    </Form.Control.Feedback>
                </Col>

                <Col lg="12">
                    <div className="d-grid">
                        <Button variant="success" type="submit">
                            <i className="bs bxs-lock-green"/>Sign Up
                        </Button>
                    </div>
                </Col>
            </Form>
        </div>
    </>
}
export default RegisterView