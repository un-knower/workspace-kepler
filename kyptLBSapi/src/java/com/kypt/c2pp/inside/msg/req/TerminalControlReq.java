package com.kypt.c2pp.inside.msg.req;

import java.io.UnsupportedEncodingException;
import java.util.HashMap;

import com.kypt.c2pp.inside.msg.InsideMsg;
import com.kypt.c2pp.util.ValidationUtil;

/**
 * 监管平台发送的终端控制指令
 */
public class TerminalControlReq extends InsideMsg {

	public static final String COMMAND = "D_CTLM_DOWN";

	private String seq;

	private String macId;

	private String commType;

	private String paramType;

	private int retry = 0;

	private HashMap hm = new HashMap();
	
	public TerminalControlReq(){
		super.setCommand(COMMAND);
	}

	public String getSeq() {
		return seq;
	}

	public void setSeq(String seq) {
		this.seq = seq;
	}

	public String getMacId() {
		return macId;
	}

	public void setMacId(String macId) {
		this.macId = macId;
	}

	public String getCommType() {
		return commType;
	}

	public void setCommType(String commType) {
		this.commType = commType;
	}

	public int getRetry() {
		return retry;
	}

	public void setRetry(int retry) {
		this.retry = retry;
	}

	public HashMap getHm() {
		return hm;
	}

	public void setHm(HashMap hm) throws Exception {
		if (ValidationUtil.validateTerminalParamId(hm)) {
			this.hm = hm;
		} else {
			throw new Exception("非法参数信息，请修改！");
		}
	}

	public void setBody(String[] msg) {
		this.seq = msg[1];
		this.macId = msg[2];
		this.commType = msg[3];

		String msgTemp[] = msg[5].substring(1, msg[5].length() - 1).split(",");

		for (int i = 0; i < msgTemp.length; i++) {
			String msgKV[] = msgTemp[i].split(":");
			if (i == 0) {
				this.paramType = msgKV[1];
			} else if (i == 1) {
				this.retry = Integer.parseInt(msgKV[1]);
			} else {
				hm.put(msgKV[0], msgKV[1]);
			}
		}

	}

	@Override
	public byte[] getBytes() throws UnsupportedEncodingException {

		String req = this.toString();
		if (this.getEncoding() != null && this.getEncoding().length() > 0) {
			return req.getBytes(this.getEncoding());
		} else {
			return req.getBytes();
		}

	}

	public String toString() {
		return "";

	}

}
