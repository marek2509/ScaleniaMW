﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    static class Constants
    {
       /* public const string podzialSekcjiNaStronieNieparzystej0 = @" \sect }\sectd \ltrsect\sbkodd\linex0\headery708\footery708\colsx708\endnhere\sectdefaultcl\sectrsid13334316\sftnbj \pard\plain \ltrpar\ql \li0\ri0\sa200\sl276\slmult1\widctlpar\wrapdefault\aspalpha\aspnum\faauto\adjustright\rin0\lin0\itap0\pararsid16596786 \rtlch\fcs1 \af0\afs22\alang1025 \ltrch\fcs0 \fs22\lang1045\langfe1045\loch\af31506\hich\af31506\dbch\af31505\cgrid\langnp1045\langfenp1045";
        public const string podzialSekcjiNaStronieNieparzystej1 = @"\sectd \ltrsect\sbkodd\linex0\headery708\footery708\colsx708\endnhere\sectlinegrid360\sectdefaultcl\sectrsid13698412\sftnbj \pard\plain \ltrpar\ql \li0\ri0\sa200\sl276\slmult1\widctlpar\wrapdefault\aspalpha\aspnum\faauto\adjustright\rin0\lin0\itap0 \rtlch\fcs1 \af31507\afs22\alang1025 \ltrch\fcs0 \f31506\fs22\lang1045\langfe1033\cgrid\langnp1045\langfenp1033";
        public const string podzialSekcjiNaStronieNieparzystej = @"{\rtlch\fcs1 \af31507 \ltrch\fcs0 \b\insrsid8918145 \sect }\sectd \ltrsect \sbkodd\linex0\headery708\footery708\colsx708\endnhere\sectlinegrid360\sectdefaultcl\sectrsid8918145\sftnbj \pard\plain \ltrpar\ql \li0\ri0\sa200\sl276\slmult1\widctlpar\wrapdefault\aspalpha\aspnum\faauto\adjustright\rin0\lin0\itap0 \rtlch\fcs1 \af31507\afs22\alang1025 \ltrch\fcs0 \f31506\fs22\lang1045\langfe1033\cgrid\langnp1045\langfenp1033 ";
        */
        //SQL
       // public const string SQLWlascicieleAdresyUdziayIdNKRNOWY = @"select jn.id_id, u.ud, u.ud_nr, case when upper(p.typ) like 'F' then (select case when dim is null then nzw || ' ' || pim else nzw || ' ' ||  pim || ' ' || dim end from osoby o where p.id_os = o.id_id) when upper(p.typ) like 'P' then(select NPE from INSTYTUCJE i where p.id_os = i.id_id) when upper(p.typ) like 'I' then (select NPE from INNE_PODM inne where p.id_os = inne.id_id) when upper(p.typ) like 'M' then (select case when maz.dim is null and zona.dim is null then 'M:' || maz.nzw  || ' '  ||  maz.pim || ' Ż:' ||  zona.nzw || ' ' || zona.pim when maz.dim is null and zona is not null then 'M:' || maz.nzw  || ' '  ||  maz.pim || ' Ż:' ||  zona.nzw || ' ' || zona.pim || ' ' ||  zona.dim when maz.dim is not null and zona.dim is null then 'M:' || maz.nzw  || ' '  ||  maz.pim || ' '  ||  maz.dim || ' Ż:' ||  zona.nzw || ' ' || zona.pim else 'M:' ||  maz.nzw  || ' '  ||  maz.pim || ' '  ||  maz.dim || ' Ż:' ||  zona.nzw || ' ' || zona.pim || ' ' ||  zona.dim end from malzenstwa m join osoby maz on maz.id_id = m.maz join osoby zona on zona.id_id = m.zona where p.id_os = m.id_id) end Wlasciciele, case when upper(p.typ) like 'F' then(select tar2 from osoby o where p.id_os = o.id_id) when upper(p.typ) like 'P' then(select tar2 from INSTYTUCJE i where p.id_os = i.id_id) when upper(p.typ) like 'I' then(select tar2 from INNE_PODM inne where p.id_os = inne.id_id) when upper(p.typ) like 'M' then(select 'M:' || maz.tar2 || ' Ż:' || zona.tar2 from malzenstwa m join osoby maz on maz.id_id = m.maz join osoby zona on zona.id_id = m.zona where p.id_os = m.id_id) end Adresy, case when upper(p.typ) like 'M' then p.id_os else 0 end malzenstwo  from udzialy_n u right join jedn_rej_n jn on jn.id_id = u.id_jedn join podmioty p on p.ID_ID = u.id_podm where jn.id_sti<> 1  or jn.id_sti is null order by ijr";
        public const string SQLWlascicieleAdresyUdziayIdNKRNOWY = @"select jn.id_id, u.ud, u.ud_nr, case when upper(p.typ) like 'F' then (select case when dim is null then nzw || ' ' || pim else nzw || ' ' ||  pim || ' ' || dim end from osoby o where p.id_os = o.id_id) when upper(p.typ) like 'P' then(select NPE from INSTYTUCJE i where p.id_os = i.id_id) when upper(p.typ) like 'I' then (select NPE from INNE_PODM inne where p.id_os = inne.id_id) when upper(p.typ) like 'M' then (select case when maz.dim is null and zona.dim is null then 'M:' || maz.nzw  || ' '  ||  maz.pim || ' Ż:' ||  zona.nzw || ' ' || zona.pim when maz.dim is null and zona is not null then 'M:' || maz.nzw  || ' '  ||  maz.pim || ' Ż:' ||  zona.nzw || ' ' || zona.pim || ' ' ||  zona.dim when maz.dim is not null and zona.dim is null then 'M:' || maz.nzw  || ' '  ||  maz.pim || ' '  ||  maz.dim || ' Ż:' ||  zona.nzw || ' ' || zona.pim else 'M:' ||  maz.nzw  || ' '  ||  maz.pim || ' '  ||  maz.dim || ' Ż:' ||  zona.nzw || ' ' || zona.pim || ' ' ||  zona.dim end from malzenstwa m join osoby maz on maz.id_id = m.maz join osoby zona on zona.id_id = m.zona where p.id_os = m.id_id) end Wlasciciele, case when upper(p.typ) like 'F' then(select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from osoby o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrl is not null then nra || '/' || nrl when nra is not null then nra else '' end from osoby o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from osoby o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from osoby o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from osoby o1 where o1.id_id = p.id_os) when upper(p.typ) like 'P' then( select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from instytucje o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrlok is not null then nra || '/' || nrlok when nra is not null then nra else '' end from instytucje o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from instytucje o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from instytucje o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from instytucje o1 where o1.id_id = p.id_os) when upper(p.typ) like 'I' then(select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from INNE_PODM o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrlok is not null then nra || '/' || nrlok when nra is not null then nra else '' end from INNE_PODM o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from INNE_PODM o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from INNE_PODM o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from INNE_PODM o1 where p.id_os = o1.id_id) when upper(p.typ) like 'M' then(select 'M:' || (select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from osoby o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrl is not null then nra || '/' || nrl when nra is not null then nra else '' end from osoby o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from osoby o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from osoby o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from osoby o1 where o1.id_id = maz.id_id) || ' Ż:' || (select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from osoby o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrl is not null then nra || '/' || nrl when nra is not null then nra else '' end from osoby o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from osoby o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from osoby o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from osoby o1 where o1.id_id = zona.id_id) from malzenstwa m join osoby maz on maz.id_id = m.maz join osoby zona on zona.id_id = m.zona where p.id_os = m.id_id) end Adresy, case when upper(p.typ) like 'M' then p.id_os else 0 end malzenstwo  from udzialy_n u right join jedn_rej_n jn on jn.id_id = u.id_jedn join podmioty p on p.ID_ID = u.id_podm where jn.id_sti<> 1  or jn.id_sti is null order by ijr";
        //public const string SQLWlascicieleAdresyUdziayIdNKRNOWY = @"select jn.id_id, u.ud, u.ud_nr, case when upper(p.typ) like 'F' then (select case when dim is null then nzw || ' ' || pim else nzw || ' ' ||  pim || ' ' || dim  end from osoby o where p.id_os = o.id_id) when upper(p.typ) like 'P' then(select NPE from INSTYTUCJE i where p.id_os = i.id_id) when upper(p.typ) like 'I' then(select NPE from INNE_PODM inne where p.id_os = inne.id_id) when upper(p.typ) like 'M' then(select 'M:' || maz.nzw  || ' '  ||  maz.pim || ' Ż:' ||  zona.nzw || ' ' || zona.pim from malzenstwa m join osoby maz on maz.id_id = m.maz join osoby zona on zona.id_id = m.zona where p.id_os = m.id_id) end Wlasciciele, case when upper(p.typ) like 'F' then(select tar2 from osoby o where p.id_os = o.id_id) when upper(p.typ) like 'P' then(select tar2 from INSTYTUCJE i where p.id_os = i.id_id) when upper(p.typ) like 'I' then(select tar2 from INNE_PODM inne where p.id_os = inne.id_id) when upper(p.typ) like 'M' then(select 'M:' || maz.tar2 || ' Ż:' || zona.tar2 from malzenstwa m join osoby maz on maz.id_id = m.maz join osoby zona on zona.id_id = m.zona where p.id_os = m.id_id) end Adresy, case when upper(p.typ) like 'M' then p.id_os else 0 end malzenstwo  from udzialy_n u right join jedn_rej_n jn on jn.id_id = u.id_jedn join podmioty p on p.ID_ID = u.id_podm where jn.id_sti<> 1  or jn.id_sti is null order by ijr";
        public const string SQLJedn_rej_N = @"select  jn.id_id, jn.ijr, jn.nkr, jn.ODCHT, jn.zgoda, jn.uwg, jn.id_obr from JEDN_REJ_N jn where jn.id_sti <> 1  or jn.id_sti is null order by ijr";
       // public const string SQLJedn_SN = @"select  jsn.id_jednn, jsn.id_jedns, jr.ijr, jr.id_obr, jsn.ud ud_z_JRs, jsn.wwgsp wrt_przed, jsn.powwgsp/10000 pow_przed from JEDN_REJ_N jn join jedn_sn jsn on jsn.id_jednn = jn.id_id join jedn_rej jr on jr.id_id = jsn.id_jedns where jn.id_sti <> 1  or jn.id_sti is null order by jn.ijr";
        public const string SQLJedn_SN = @"select jsn.id_jednn, jsn.id_jedns, jr.ijr, jr.id_obr, jsn.ud ud_z_JRs, jsn.wwgsp wrt_przed, jsn.powwgsp/10000 pow_przed from JEDN_REJ_N jn join jedn_sn jsn on jsn.id_jednn = jn.id_id join jedn_rej jr on jr.id_id = jsn.id_jedns join obreby o on o.id_id = jr.id_obr where jn.id_sti<> 1  or jn.id_sti is null order by  jn.ijr, o.naz, jr.ijr";

        public const string SQL_Obreby = @"select id_id, id, naz from obreby";
        public const string SQL_Dzialki_N = @"select id_id, idobr, idd, pew/10000 PEW, rjdr, rjdrprzed, kw, ww from dzialki_n order by idobr, idd";
        //public const string SQL_Dzialka = @"select id_id, idobr, idd, pew/10000 PEW, rjdr, kw, ww from dzialka where id_sti is null or id_sti<> 1";
       // public const string SQL_WlascicielAdresyUdzialyIdNkrStary = @"select js.id_id, u.ud, u.ud_nr, case when upper(p.typ) like 'F' then (select case when dim is null then nzw || ' ' || pim else nzw || ' ' ||  pim || ' ' || dim end from osoby o where p.id_os = o.id_id) when upper(p.typ) like 'P' then(select NPE from INSTYTUCJE i where p.id_os = i.id_id) when upper(p.typ) like 'I' then (select NPE from INNE_PODM inne where p.id_os = inne.id_id) when upper(p.typ) like 'M' then (select case when maz.dim is null and zona.dim is null then 'M:' || maz.nzw  || ' '  ||  maz.pim || ' Ż:' ||  zona.nzw || ' ' || zona.pim when maz.dim is null and zona is not null then 'M:' || maz.nzw  || ' '  ||  maz.pim || ' Ż:' ||  zona.nzw || ' ' || zona.pim || ' ' ||  zona.dim when maz.dim is not null and zona.dim is null then 'M:' || maz.nzw  || ' '  ||  maz.pim || ' '  ||  maz.dim || ' Ż:' ||  zona.nzw || ' ' || zona.pim else 'M:' ||  maz.nzw  || ' '  ||  maz.pim || ' '  ||  maz.dim || ' Ż:' ||  zona.nzw || ' ' || zona.pim || ' ' ||  zona.dim end from malzenstwa m join osoby maz on maz.id_id = m.maz join osoby zona on zona.id_id = m.zona where p.id_os = m.id_id) end Wlasciciele, case when upper(p.typ) like 'F' then(select tar2 from osoby o where p.id_os = o.id_id) when upper(p.typ) like 'P' then(select tar2 from INSTYTUCJE i where p.id_os = i.id_id) when upper(p.typ) like 'I' then(select tar2 from INNE_PODM inne where p.id_os = inne.id_id) when upper(p.typ) like 'M' then(select 'M:' || maz.tar2 || ' Ż:' || zona.tar2 from malzenstwa m join osoby maz on maz.id_id = m.maz join osoby zona on zona.id_id = m.zona where p.id_os = m.id_id) end Adresy, case when upper(p.typ) like 'M' then p.id_os else 0 end malzenstwo from udzialy u right join jedn_rej js on js.id_id = u.id_jedn join podmioty p on p.ID_ID = u.id_podm where js.id_sti<> 1  or js.id_sti is null order by ijr";
        public const string SQL_WlascicielAdresyUdzialyIdNkrStary = @"select js.id_id, u.ud, u.ud_nr, case when upper(p.typ) like 'F' then (select case when dim is null then nzw || ' ' || pim else nzw || ' ' ||  pim || ' ' || dim end from osoby o where p.id_os = o.id_id) when upper(p.typ) like 'P' then(select NPE from INSTYTUCJE i where p.id_os = i.id_id) when upper(p.typ) like 'I' then (select NPE from INNE_PODM inne where p.id_os = inne.id_id) when upper(p.typ) like 'M' then (select case when maz.dim is null and zona.dim is null then 'M:' || maz.nzw  || ' '  ||  maz.pim || ' Ż:' ||  zona.nzw || ' ' || zona.pim when maz.dim is null and zona is not null then 'M:' || maz.nzw  || ' '  ||  maz.pim || ' Ż:' ||  zona.nzw || ' ' || zona.pim || ' ' ||  zona.dim when maz.dim is not null and zona.dim is null then 'M:' || maz.nzw  || ' '  ||  maz.pim || ' '  ||  maz.dim || ' Ż:' ||  zona.nzw || ' ' || zona.pim else 'M:' ||  maz.nzw  || ' '  ||  maz.pim || ' '  ||  maz.dim || ' Ż:' ||  zona.nzw || ' ' || zona.pim || ' ' ||  zona.dim end from malzenstwa m join osoby maz on maz.id_id = m.maz join osoby zona on zona.id_id = m.zona where p.id_os = m.id_id) end Wlasciciele, case when upper(p.typ) like 'F' then(select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from osoby o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrl is not null then nra || '/' || nrl when nra is not null then nra else '' end from osoby o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from osoby o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from osoby o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from osoby o1 where o1.id_id = p.id_os) when upper(p.typ) like 'P' then( select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from instytucje o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrlok is not null then nra || '/' || nrlok when nra is not null then nra else '' end from instytucje o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from instytucje o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from instytucje o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from instytucje o1 where o1.id_id = p.id_os) when upper(p.typ) like 'I' then(select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from INNE_PODM o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrlok is not null then nra || '/' || nrlok when nra is not null then nra else '' end from INNE_PODM o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from INNE_PODM o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from INNE_PODM o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from INNE_PODM o1 where p.id_os = o1.id_id) when upper(p.typ) like 'M' then(select 'M:' || (select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from osoby o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrl is not null then nra || '/' || nrl when nra is not null then nra else '' end from osoby o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from osoby o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from osoby o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from osoby o1 where o1.id_id = maz.id_id) || ' Ż:' || (select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from osoby o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrl is not null then nra || '/' || nrl when nra is not null then nra else '' end from osoby o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from osoby o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from osoby o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from osoby o1 where o1.id_id = zona.id_id) from malzenstwa m join osoby maz on maz.id_id = m.maz join osoby zona on zona.id_id = m.zona where p.id_os = m.id_id) end Adresy, case when upper(p.typ) like 'M' then p.id_os else 0 end malzenstwo from udzialy u right join jedn_rej js on js.id_id = u.id_jedn join podmioty p on p.ID_ID = u.id_podm where js.id_sti<> 1  or js.id_sti is null order by ijr";
        public const string SQL_Dzialka =  @"select d.id_id, d.idobr, d.idd, d.pew/10000 PEW, d.rjdr, d.kw, d.ww, case when position('.' in d.idd) <> 0 then cast(substring(substring(idd from position ('/' in d.idd)+1 for CHAR_LENGTH(idd)) from position('.' in substring(idd from position ('/' in d.idd)+1 for CHAR_LENGTH(idd)))+1) as int) when position('/' in d.idd) = 0 then cast(d.idd as int) else cast(substring(idd from 1 for position ('/' in d.idd)-1) as int) end Gl_NR, case when position('/' in d.idd) = 0 then 0 else cast(substring(idd from position ('/' in d.idd)+1 for CHAR_LENGTH(idd)) as int) end UlameKDzialki from dzialka d join obreby o on o.id_id = d.idobr where d.id_sti is null or d.id_sti<> 1 order by o.naz, Gl_NR, ulamekdzialki";
         }
}
