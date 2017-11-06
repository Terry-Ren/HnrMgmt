import qiniu from 'qiniu'

var accessKey = 'GfDe7p6rupDyEHYhvHfO-NhXvJ0KtxVOBSKxiKtU';
var secretKey = 'GfDe7p6rupDyEHYhvHfO-NhXvJ0KtxVOBSKxiKtU';
var mac = new qiniu.auth.digest.Mac(accessKey, secretKey);

//自定义凭证有效期（示例2小时，expires单位为秒，为上传凭证的有效时间）
var options = {
    scope: bucket,
    expires: 7200
};
var putPolicy = new qiniu.rs.PutPolicy(options);
var uploadToken = putPolicy.uploadToken(mac);
var putPolicy = new qiniu.rs.PutPolicy(options);

console.log(putPolicy.uploadToken(mac));
