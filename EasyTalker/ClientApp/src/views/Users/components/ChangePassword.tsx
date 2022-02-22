// @ts-ignore
import styles from './ChangePassword.module.css'
import React, { useState } from 'react'
import * as Yup from 'yup'
import { useFormik } from 'formik'
import { changePassword } from '../../../store/users/api'
import { useDispatch } from 'react-redux'
import { getLoggedUserId } from '../../../utils/authUtils'
import ChangePasswordFactors from '../../../interfaces/Users/ChangePasswordFactors'
import { Button, Col, Form } from 'react-bootstrap'
import { logout } from '../../../store/userSession/api'

interface Props {
    onSuccessful: () => void
}

const ChangePassword: React.FC<Props> = (props) => {
    
    const dispatch = useDispatch()
    
    const getInitialValues = () => {
        return {
            currentPassword: '',
            newPassword: '',
            newPasswordConfirmation: ''
        }
    }

    const getRegisterValidationSchema = () => Yup.object().shape({
        currentPassword: Yup.string().required('Current password is required'),
        newPassword: Yup.string().required('Password is required'),
        newPasswordConfirmation: Yup.string().oneOf([Yup.ref('newPassword'), null], 'Passwords must match').required('Confirm password is required')
    })

    const formik = useFormik({
        initialValues: getInitialValues(),
        validationSchema: getRegisterValidationSchema(),
        validateOnBlur: false,
        validateOnChange: false,
        onSubmit: (values) => {
            dispatch(changePassword({
                userId: getLoggedUserId(),
                changePassword: {
                    currentPassword: values.currentPassword,
                    newPassword: values.newPassword
                } as ChangePasswordFactors,
                onSuccessfulResponse: props.onSuccessful
            }))
        }
    })
    
    const onConfirmLogout = () => {
        dispatch(logout())
    }
    
    return <>
        <div className={styles.formBody}>
            <Form className={`row g-3 ${styles.formRow}`} onSubmit={formik.handleSubmit}>
                <Col lg="12">
                    <Form.Label>Current Password</Form.Label>
                    <Form.Control type="password"
                                  id="currentPassword"
                                  value={formik.values.currentPassword}
                                  isInvalid={!!formik.errors.currentPassword}
                                  placeholder="Enter password"
                                  onChange={formik.handleChange}
                    />
                    <Form.Control.Feedback type="invalid">
                        {formik.errors.currentPassword}
                    </Form.Control.Feedback>
                </Col>

                <Col lg="12">
                    <Form.Label>New Password</Form.Label>
                    <Form.Control type="password"
                                  id="newPassword"
                                  value={formik.values.newPassword}
                                  isInvalid={!!formik.errors.newPassword}
                                  placeholder="Enter password"
                                  onChange={formik.handleChange}
                    />
                    <Form.Control.Feedback type="invalid">
                        {formik.errors.newPassword}
                    </Form.Control.Feedback>
                </Col>

                <Col lg="12">
                    <Form.Label>Confirm New Password</Form.Label>
                    <Form.Control type="password"
                                  id="newPasswordConfirmation"
                                  value={formik.values.newPasswordConfirmation}
                                  isInvalid={!!formik.errors.newPasswordConfirmation}
                                  placeholder="Confirm password"
                                  onChange={formik.handleChange}
                    />
                    <Form.Control.Feedback type="invalid">
                        {formik.errors.newPasswordConfirmation}
                    </Form.Control.Feedback>
                </Col>

                <Col lg="12">
                    <div className="d-grid">
                        <Button variant="primary" type="submit">
                            <i className="bs bxs-lock-green"/>Change
                        </Button>
                    </div>
                </Col>
            </Form>
        </div>
    </>
}
export default ChangePassword