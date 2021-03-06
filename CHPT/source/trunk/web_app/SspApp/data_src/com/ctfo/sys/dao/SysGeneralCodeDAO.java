package com.ctfo.sys.dao;

import com.ctfo.sys.beans.SysGeneralCode;

public interface SysGeneralCodeDAO {

	/**
	 * This method was generated by Abator for iBATIS. This method corresponds to the database table SYS_GENERAL_CODE
	 * @abatorgenerated  Mon Mar 24 16:46:47 CST 2014
	 */
	void insert(SysGeneralCode record);

	/**
	 * This method was generated by Abator for iBATIS. This method corresponds to the database table SYS_GENERAL_CODE
	 * @abatorgenerated  Mon Mar 24 16:46:47 CST 2014
	 */
	int updateByPrimaryKey(SysGeneralCode record);

	/**
	 * This method was generated by Abator for iBATIS. This method corresponds to the database table SYS_GENERAL_CODE
	 * @abatorgenerated  Mon Mar 24 16:46:47 CST 2014
	 */
	int updateByPrimaryKeySelective(SysGeneralCode record);

	/**
	 * This method was generated by Abator for iBATIS. This method corresponds to the database table SYS_GENERAL_CODE
	 * @abatorgenerated  Mon Mar 24 16:46:47 CST 2014
	 */
	SysGeneralCode selectByPrimaryKey(String autoId);

	/**
	 * This method was generated by Abator for iBATIS. This method corresponds to the database table SYS_GENERAL_CODE
	 * @abatorgenerated  Mon Mar 24 16:46:47 CST 2014
	 */
	int deleteByPrimaryKey(String autoId);

}