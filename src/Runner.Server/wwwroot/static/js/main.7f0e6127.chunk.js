(this["webpackJsonpreact-master-detail"]=this["webpackJsonpreact-master-detail"]||[]).push([[0],{20:function(e,t,n){e.exports={component:"MasterDetail_component__Uz_TT",master:"MasterDetail_master__25jd3",detail:"MasterDetail_detail__1_NEc"}},22:function(e,t,n){e.exports={header:"Header_header__Nww_e",back:"Header_back__2pYLZ"}},25:function(e,t,n){e.exports={component:"ListItem_component__1tRbh",inner:"ListItem_inner__3kPOv"}},26:function(e,t,n){e.exports={component:"ListItemLink_component__5N6-o",active:"ListItemLink_active__14SJk"}},40:function(e,t,n){},55:function(e,t,n){"use strict";n.r(t);var r=n(0),a=n.n(r),s=n(32),i=n.n(s),c=(n(40),n(10)),o=n(4),l=n(19),u="(max-width: 599px)",d=n(22),b=n.n(d),j=n(1),p=function(e){return Object(j.jsxs)("div",{className:b.a.header,children:[Object(j.jsx)(l.a,{query:u,children:function(t){return t?Object(j.jsx)(c.b,{to:"../../",className:b.a.back,style:{visibility:e.hideBackButton?"hidden":"visible"},children:"Back"}):Object(j.jsx)("div",{children:"\xa0"})}}),Object(j.jsx)("h1",{"data-test":"HeaderTitle",children:e.title||"No Title"})]})};p.defaultProps={hideBackButton:!1};var f=n(25),h=n.n(f),m="No Data",x=function(e){return Object(j.jsx)("div",{className:h.a.component,children:Object(j.jsxs)("div",{className:h.a.inner,children:[Object(j.jsx)("h1",{"data-test":"ListItemHeading",children:e.item.title?e.item.title:m}),Object(j.jsx)("h2",{"data-test":"ListItemSubHeading",children:e.item.description?e.item.description:m})]})})},O=n(15),g=n(26),v=n.n(g),_=function(e){return Object(j.jsx)(c.c,{exact:!0,to:e.to,className:v.a.component,activeClassName:v.a.active,children:Object(j.jsx)(x,Object(O.a)({},e))})},w=n(20),k=n.n(w),I=function(e){var t=Object(o.h)().path,n=Object(j.jsx)(e.MasterType,Object(O.a)(Object(O.a)({},e.masterProps),{},{"data-test":"Master"})),r=Object(j.jsx)(e.DetailType,Object(O.a)(Object(O.a)({},e.detailProps),{},{"data-test":"Detail"}));return Object(j.jsx)(l.a,{query:u,children:function(e){return e?Object(j.jsxs)(o.d,{children:[Object(j.jsx)(o.b,{exact:!0,path:"".concat(t),children:n}),Object(j.jsx)(o.b,{path:"".concat(t,"/detail/:id"),children:r})]}):Object(j.jsxs)("section",{className:k.a.component,children:[Object(j.jsx)("section",{className:k.a.master,children:Object(j.jsx)(o.b,{path:"".concat(t),children:n})}),Object(j.jsx)("section",{className:k.a.detail,children:Object(j.jsxs)(o.d,{children:[Object(j.jsx)(o.b,{exact:!0,path:"".concat(t),children:r}),Object(j.jsx)(o.b,{path:"".concat(t,"/detail/:id"),children:r})]})})]})}})},N=n(12),L=n(5),C=n.n(L),y=n(13),S=n(11),T=function(e,t){var n="string"===typeof t?parseInt(t,10):t;if(void 0!==n&&null!==n){var r=e.find((function(e,t,r){return e.requestId===n}))||null;if(null!==r)return{item:{id:r.requestId,title:r.jobId,description:r.timeLineId},job:r}}return{item:null,job:null}},q=n(9),D=n.n(q),M=n(27),E=n.n(M),H=n(35),J=n.n(H);function B(e){return P.apply(this,arguments)}function P(){return(P=Object(S.a)(C.a.mark((function e(t){var n,r,a;return C.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return n="/runner/host/_apis/pipelines/workflows/"+t+"/artifacts",e.next=3,fetch(n);case 3:return r=e.sent,e.next=6,r.text();case 6:return a=e.sent,e.abrupt("return",JSON.parse(a));case 8:case"end":return e.stop()}}),e)})))).apply(this,arguments)}function W(e,t){return R.apply(this,arguments)}function R(){return(R=Object(S.a)(C.a.mark((function e(t,n){var r,a,s;return C.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return(r=new URL(n)).searchParams.append("itemPath",t),e.next=4,fetch(r.toString());case 4:return a=e.sent,e.next=7,a.text();case 7:return s=e.sent,e.abrupt("return",JSON.parse(s));case 9:case"end":return e.stop()}}),e)})))).apply(this,arguments)}var F=function(e){var t=Object(r.useState)([]),n=Object(y.a)(t,2),a=n[0],s=n[1],i=Object(r.useState)([]),c=Object(y.a)(i,2),l=c[0],u=c[1],d=Object(r.useState)([]),b=Object(y.a)(d,2),f=b[0],h=b[1],m=Object(r.useState)("Loading..."),x=Object(y.a)(m,2),O=x[0],g=x[1],v=Object(o.g)().id,_=Object(o.g)(),w=_.owner,k=_.repo,I=Object(r.useState)([]),L=Object(y.a)(I,2),q=L[0],M=L[1];return Object(r.useEffect)((function(){Object(S.a)(C.a.mark((function e(){var t,n,r,i,c,o,l,d,b,j,p;return C.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:if(h((function(e){return[]})),void 0!==v){e.next=3;break}return e.abrupt("return");case 3:if(n=Number.parseInt(v),0!==a.length&&null!=a.find((function(e){return e.requestId===n}))){e.next=11;break}return e.next=7,fetch("/"+w+"/"+k+"/_apis/v1/Message",{});case 7:return e.next=9,e.sent.json();case 9:t=e.sent,s(t);case 11:if(null!==(r=T(t||a,v)).job.errors&&r.job.errors.length>0?M(r.job.errors):M([]),i=r.item,null==(c=i?i.description:null)){e.next=28;break}return e.next=18,fetch("/"+w+"/"+k+"/_apis/v1/Timeline/"+c,{});case 18:if(200!==(o=e.sent).status){e.next=26;break}return e.next=22,o.json();case 22:null!=(l=e.sent)&&l.length>1?(g(l.shift().name),u(l)):(g("Unknown"),u([])),e.next=28;break;case 26:g(null!==r.job.errors&&r.job.errors.length>0?"Failed to run":"Wait for workflow to run..."),u((function(e){return[]}));case 28:if(-1===r.job.runid){e.next=44;break}return e.next=31,B(r.job.runid);case 31:if(void 0===(d=e.sent).value){e.next=44;break}b=0;case 34:if(!(b<d.count)){e.next=43;break}return j=d.value[b],e.next=38,W(j.name,j.fileContainerResourceUrl);case 38:void 0!==(p=e.sent)&&(j.files=p.value);case 40:b++,e.next=34;break;case 43:h((function(e){return d.value}));case 44:case"end":return e.stop()}}),e)})))()}),[v,a,w,k]),Object(r.useEffect)((function(){if(void 0!==v&&null!==v&&v.length>0){var e=T(a,v).item;if(null!==e){var t=new EventSource("/"+w+"/"+k+"/_apis/v1/TimeLineWebConsoleLog?timelineId="+e.description),n=[],r=function(t,n){var r=t.find((function(e){return e.id===n.record.stepId})),a=new E.a({newline:!0,escapeXML:!0});return null!=r&&void 0!=r&&(null==r.log&&(r.log={id:-1,location:null,content:""},n.record.startLine>1&&Object(S.a)(C.a.mark((function t(){var s,i;return C.a.wrap((function(t){for(;;)switch(t.prev=t.next){case 0:return console.log("Downloading previous log lines of this step..."),t.next=3,fetch("/"+w+"/"+k+"/_apis/v1/TimeLineWebConsoleLog/"+e.description+"/"+n.record.stepId,{});case 3:if(200!==(s=t.sent).status){t.next=12;break}return t.next=7,s.json();case 7:(i=t.sent).length=n.record.startLine-1,r.log.content=i.reduce((function(e,t){return(e.length>0?e+"<br/>":"")+a.toHtml(t.line)}),"")+r.log.content,t.next=13;break;case 12:console.log("No logs to download..., currently fixes itself");case 13:case"end":return t.stop()}}),t)})))()),-1===r.log.id&&(r.log.content=n.record.value.reduce((function(e,t){return(e.length>0?e+"<br/>":"")+a.toHtml(t)}),r.log.content)),!0)};return t.addEventListener("log",(function(e){console.log("new logline "+e.data);var t=JSON.parse(e.data);u((function(e){return r(e,t)?Object(N.a)(e):(n.push(t),e)}))})),t.addEventListener("timeline",(function(e){var t=JSON.parse(e.data);g(t.timeline.shift().name),u((function(e){for(var a=t.timeline.splice(0,e.length),s=0;s<a.length;s++)e[s].result=a[s].result,e[s].state=a[s].state;if(0===t.timeline.length)return e;for(var i=[].concat(Object(N.a)(e),Object(N.a)(t.timeline));n.length>0&&r(i,n[0]);)n.shift();return i}))})),function(){t.close()}}}return function(){}}),[v,a,w,k]),Object(j.jsxs)("section",{className:D.a.component,children:[Object(j.jsx)(p,{title:O}),Object(j.jsx)("main",{className:D.a.main,children:Object(j.jsxs)("div",{className:D.a.text,style:{width:"100%"},children:[function(){var e=T(a,v);return void 0===e||null==e.job||e.job.cancelRequest?Object(j.jsx)("div",{children:"This Job was cancelled"}):Object(j.jsx)("button",{onClick:function(t){Object(S.a)(C.a.mark((function t(){return C.a.wrap((function(t){for(;;)switch(t.prev=t.next){case 0:return t.next=2,fetch("/"+w+"/"+k+"/_apis/v1/Message/cancel/"+e.job.jobId,{method:"POST"});case 2:case"end":return t.stop()}}),t)})))()},children:"Cancel"})}(),q.map((function(e){return Object(j.jsxs)("div",{children:["Error: ",e]})})),f.map((function(e){return Object(j.jsxs)("div",{children:[Object(j.jsx)("div",{children:e.name}),void 0!==e.files?e.files.map((function(e){return Object(j.jsx)("div",{children:Object(j.jsx)("a",{href:e.contentLocation,children:e.path})})})):Object(j.jsx)("div",{})]})})),l.map((function(e){return Object(j.jsx)(J.a,{className:D.a.Collapsible,openedClassName:D.a.Collapsible,triggerClassName:D.a.Collapsible__trigger,triggerOpenedClassName:D.a.Collapsible__trigger+" "+D.a["is-open"],contentOuterClassName:D.a.Collapsible__contentOuter,contentInnerClassName:D.a.Collapsible__contentInner,trigger:(null==e.result?null==e.state?"Waiting":e.state:e.result)+" - "+e.name,onOpening:function(){e.busy||null!=e.log&&(-1===e.log.id||e.log.content&&0!==e.log.content.length)||(e.busy=!0,Object(S.a)(C.a.mark((function t(){var n,r,s,i,c,o,l,d;return C.a.wrap((function(t){for(;;)switch(t.prev=t.next){case 0:if(t.prev=0,n=new E.a({newline:!0,escapeXML:!0}),null!=e.log){t.next=18;break}return console.log("Downloading previous log lines of this step..."),r=T(a,v).item,t.next=7,fetch("/"+w+"/"+k+"/_apis/v1/TimeLineWebConsoleLog/"+r.description+"/"+e.id,{});case 7:if(200!==(s=t.sent).status){t.next=15;break}return t.next=11,s.json();case 11:i=t.sent,e.log={id:-1,location:null,content:i.reduce((function(e,t){return(e.length>0?e+"<br/>":"")+n.toHtml(t.line)}),"")},t.next=16;break;case 15:console.log("No logs to download...");case 16:t.next=28;break;case 18:return t.next=20,fetch("/"+w+"/"+k+"/_apis/v1/Logfiles/"+e.log.id,{});case 20:return t.next=22,t.sent.text();case 22:c=t.sent,o=c.split("\n"),l="2021-04-02T15:50:14.6619714Z ".length,d=/^[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}.[0-9]{7}Z /,o[0]=n.toHtml(d.test(o[0])?o[0].substring(l):o[0]),e.log.content=o.reduce((function(e,t){return(e.length>0?e+"<br/>":"")+n.toHtml(d.test(t)?t.substring(l):t)}));case 28:return t.prev=28,e.busy=!1,u((function(e){return Object(N.a)(e)})),t.finish(28);case 32:case"end":return t.stop()}}),t,null,[[0,,28,32]])})))())},children:Object(j.jsx)("div",{style:{textAlign:"left",whiteSpace:"nowrap",maxHeight:"100%",overflow:"auto",fontFamily:"SFMono-Regular,Consolas,Liberation Mono,Menlo,monospace"},dangerouslySetInnerHTML:{__html:null!=e.log?e.log.content:"Nothing here"}})},v+e.id)}))]})})]})},U=function(e){var t=Object(o.h)().url,n=Object(r.useState)([]),s=Object(y.a)(n,2),i=s[0],c=s[1],l=Object(o.g)(),u=l.owner,d=l.repo;return Object(r.useEffect)((function(){new EventSource("/"+u+"/"+d+"/_apis/v1/Message/event?filter=**").addEventListener("job",(function(e){var t=JSON.parse(e.data).job;c((function(e){var n=e.filter((function(e){return e.requestId<t.requestId}));return n.unshift(t),n}))}));var e="/"+u+"/"+d+"/_apis/v1/Message";Object(S.a)(C.a.mark((function t(){var n,r;return C.a.wrap((function(t){for(;;)switch(t.prev=t.next){case 0:return t.t0=JSON,t.next=3,fetch(e,{});case 3:return t.next=5,t.sent.text();case 5:t.t1=t.sent,n=t.t0.parse.call(t.t0,t.t1),r=n.sort((function(e,t){return t.requestId-e.requestId})),c((function(e){if(e.length>0){var t=e[e.length-1];r=r.filter((function(e){return e.requestId<t.requestId}))}return[].concat(Object(N.a)(r),Object(N.a)(e))}));case 9:case"end":return t.stop()}}),t)})))()}),[u,d]),Object(j.jsxs)(a.a.Fragment,{children:[Object(j.jsx)(p,{title:"Jobs",hideBackButton:!0}),Object(j.jsx)("ul",{children:i.map((function(e){return Object(j.jsx)("li",{children:Object(j.jsx)(_,{to:"".concat(t,"/detail/").concat(e.requestId),item:{id:e.requestId,title:e.name,description:e.workflowname+" - "+e.repo+" - "+e.requestId}})},e.requestId)}))})]})},X=function(){return Object(j.jsx)(c.a,{children:Object(j.jsxs)(o.d,{children:[Object(j.jsx)(o.b,{path:"/master/:owner/:repo",render:function(e){return Object(j.jsx)(I,{MasterType:U,masterProps:{},DetailType:F,detailProps:{}})}}),Object(j.jsx)(o.a,{exact:!0,from:"/",to:"/master/runner/server"})]})})};Boolean("localhost"===window.location.hostname||"[::1]"===window.location.hostname||window.location.hostname.match(/^127(?:\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$/));i.a.render(Object(j.jsx)(X,{}),document.getElementById("root")),"serviceWorker"in navigator&&navigator.serviceWorker.ready.then((function(e){e.unregister()}))},9:function(e,t,n){e.exports={component:"Detail_component__1-q_u",main:"Detail_main__2xXle",Collapsible:"Detail_Collapsible__2slk1",Collapsible__contentInner:"Detail_Collapsible__contentInner__3s9IG",Collapsible__trigger:"Detail_Collapsible__trigger__O7Wcx","is-open":"Detail_is-open__2vygp","is-disabled":"Detail_is-disabled__2L3mn","Collapsible__custom-sibling":"Detail_Collapsible__custom-sibling__1TuEJ"}}},[[55,1,2]]]);
//# sourceMappingURL=main.7f0e6127.chunk.js.map